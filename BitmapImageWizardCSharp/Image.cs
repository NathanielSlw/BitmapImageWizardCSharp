using System;
using System.Globalization;
using System.IO;
using System.Net;
using System.Runtime.ConstrainedExecution;
using static System.Net.Mime.MediaTypeNames;

namespace ProjetImage
{
	public class Image
	{
        
        string typeImg;
        int hauteurImg;
		int largeurImg;
        int tailleFichier;
        int tailleOffset;
        Pixel[,] matrice;
        int bitsParPixel;


        // Constructeur pour le fractale
        public Image(int hauteurImg, int largeurImg)
        {
            this.typeImg = "6677";
            this.hauteurImg = hauteurImg;
            this.largeurImg = largeurImg;
            this.tailleFichier = (hauteurImg*largeurImg)*3 + 54;
            this.tailleOffset = 0;
            this.matrice = new Pixel[hauteurImg, largeurImg];
            this.bitsParPixel = 24;
        }

		public Image(string nomFile)
		{
            byte[] myfile = File.ReadAllBytes(nomFile);
            
            if (myfile[0] == 66 && myfile[1] == 77)
            {
                typeImg = "BM";
            }
            for (int i = 0; i <= 3; i++)
            {
                tailleFichier += Convert.ToInt32(myfile[i + 2] * Math.Pow(256, i));
                largeurImg += Convert.ToInt32(myfile[i + 18] * Math.Pow(256, i));
                hauteurImg += Convert.ToInt32(myfile[i + 22] * Math.Pow(256, i));
                //tailleOffset += Convert.ToInt32(myfile[i + 31] * Math.Pow(256, i));
                bitsParPixel += Convert.ToInt32(myfile[i + 28] * Math.Pow(256, i));
            }
            tailleOffset = Convert.ToInt32(myfile[31]) + Convert.ToInt32(myfile[32]) * 256 + Convert.ToInt32(myfile[33]) * 256 * 256 + Convert.ToInt32(myfile[34]) * 256 * 256 * 256;
            this.matrice = TransformerEnMatrice(myfile);
            
            
        }

        // Constructeur pour aggrandir une image
        public Image(string nomFile, int agrandissement)
        {
            byte[] myfile = File.ReadAllBytes(nomFile);

            if (myfile[0] == 66 && myfile[1] == 77)
            {
                typeImg = "BM";
            }
            for (int i = 0; i <= 3; i++)
            {
                tailleFichier += Convert.ToInt32(myfile[i + 2] * Math.Pow(256, i));
                largeurImg += Convert.ToInt32(myfile[i + 18] * Math.Pow(256, i));
                hauteurImg += Convert.ToInt32(myfile[i + 22] * Math.Pow(256, i));
                //tailleOffset += Convert.ToInt32(myfile[i + 31] * Math.Pow(256, i));
                bitsParPixel += Convert.ToInt32(myfile[i + 28] * Math.Pow(256, i));
            }
            tailleOffset = Convert.ToInt32(myfile[31]) + Convert.ToInt32(myfile[32]) * 256 + Convert.ToInt32(myfile[33]) * 256 * 256 + Convert.ToInt32(myfile[34]) * 256 * 256 * 256;
            
            this.matrice = TransformerEnMatrice(myfile);
            
            this.matrice = Agrandissement(agrandissement);
            largeurImg = largeurImg * agrandissement;
            hauteurImg = hauteurImg * agrandissement;
            tailleFichier = 0;
            tailleFichier = 54 + (largeurImg * hauteurImg * (bitsParPixel / 8));


        }
        

        // Conversion d'un tableau de byte en un entier
        public int Convertir_Endian_To_Int(byte[] tab)
        {
            int res = 0;
            for(int i=0; i<tab.Length; i++)
            {
                res += Convert.ToInt32(tab[i] * Math.Pow(256, i));
            }
            return res;
        }

        // Conversion d'un entier en tableau de byte
        public byte[] Convertir_Int_To_Endian(int val)
        {
            byte[] converti = BitConverter.GetBytes(val);
            return converti;
        }
       

        public Pixel[,] TransformerEnMatrice(byte[] myfile)
        {
            // Fais commencer le tableau à l'indice 0 pour que ce soit plus simple
            // pour la fonction TransformerEnMatrice
            byte[] img = new byte[myfile.Length - 54];
            for (int i = 54; i < myfile.Length; i++)
            {
                img[i - 54] = myfile[i];
            }
           
            Pixel[,] matrice = new Pixel[hauteurImg, largeurImg];
            for(int i=0; i<matrice.GetLength(0); i++)
            {
                for(int j=0; j<matrice.GetLength(1); j++)
                {
                    // matrice(i, j) = tab(i*largeurImg + j)*3
                    Pixel P = new Pixel(0, 0, 0);
                    int pos = (i * largeurImg + j) * 3;
                    P.B = img[pos];
                    P.G = img[pos + 1];
                    P.R = img[pos + 2];
                    matrice[i, j] = P;
                }
            }

            return matrice;
        }

        public void AfficherMatriceImage()
        {
            for (int i = 0; i < matrice.GetLength(0); i++)
            {
                for (int j = 0; j < matrice.GetLength(1); j++)
                {
                    Console.Write(matrice[i, j].B + " " + matrice[i, j].G + " " + matrice[i, j].R + " ");
                }
                Console.WriteLine();
            }
        }

        // transforme la matrice en tableau de bytes
        public void FromImageToFile(string nomFile)
        {
            byte[] myfile = File.ReadAllBytes(nomFile);

            byte[] tab = new byte[tailleFichier];
            // Header + Metadonnées
           
            
            for (int i = 0; i < 54; i++) { tab[i] = myfile[i]; }

            //taille fichier à convertir
            byte[] t_fichier = Convertir_Int_To_Endian(tailleFichier);
            for (int i = 0; i < t_fichier.Length; i++)
            {
                tab[i + 2] = t_fichier[i];
            }

            //largeur image à convertir
            byte[] lar_img = Convertir_Int_To_Endian(largeurImg);
            for (int i = 0; i < lar_img.Length; i++)
            {
                tab[i + 18] = lar_img[i];
            }

            //hauteur image à convertir
            byte[] haut_img = Convertir_Int_To_Endian(hauteurImg);
            for (int i = 0; i < haut_img.Length; i++)
            {
                tab[i + 22] = haut_img[i];
            }

            //taille offset à convertir
            byte[] t_offset = Convertir_Int_To_Endian(tailleOffset);
            for (int i = 0; i < t_offset.Length; i++)
            {
                tab[i + 31] = t_offset[i];
            }
           
            // matrice(i, j) = tab(i*largeurImg + j)*3
            for (int i = 0; i < matrice.GetLength(0); i++)
            {
                for (int j = 0; j < matrice.GetLength(1); j++)
                {

                   
                    tab[((i * largeurImg) + j)*3+ 54] = matrice[i, j].B;
                    tab[((i * largeurImg) + j)*3 + 55] = matrice[i, j].G;
                    tab[((i * largeurImg) + j)*3 + 56] = matrice[i, j].R;
                }
               
            }

            File.WriteAllBytes("./Images/Sortie.bmp", tab);
            
        }


        public void MettreEnGris()
        {

            if (matrice != null)
            {

                for (int i = 0; i < matrice.GetLength(0); i++)
                {
                    for (int j = 0; j < matrice.GetLength(1); j++)
                    {

                        byte gris = (byte)((matrice[i, j].R + matrice[i, j].G + matrice[i, j].B) / 3);
                        matrice[i, j].R = gris;
                        matrice[i, j].G = gris;
                        matrice[i, j].B = gris;
                    }
                }

            }

        }

        // Méthode pour faire la rotation d'un angle donné en degrés
        public Pixel[,] Rotate(double angle)
        {
            int width = matrice.GetLength(0);
            int height = matrice.GetLength(1);

            // Convertir l'angle en radians
            double radians = angle * Math.PI / 180;

            // Calculer le cosinus et le sinus de l'angle
            double cos = Math.Cos(radians);
            double sin = Math.Sin(radians);

            // Initialiser la nouvelle matrice de pixels
            Pixel[,] matriceRotate = new Pixel[width, height];

            // Parcourir la matrice originale et remplir la nouvelle matrice en appliquant la rotation
            int newX = 0;
            int newY = 0;
            double x0 = width / 2.0;
            double y0 = height / 2.0;

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    // Calculer les coordonnées x et y de chaque pixel après la rotation
                    // Formule : nouveauX(x-Rx)*cos - (y-Ry)*sin + Rx
                    // neauveauY = (x-Rx)*sin + (y-Ry)*cos + Ry
                    newX = (int)Math.Round((x - x0) * cos - (y - y0) * sin + x0);
                    newY = (int)Math.Round((x - x0) * sin + (y - y0) * cos + y0);

                    // Vérifier que les coordonnées calculées sont bien dans la matrice
                    if (newX >= 0 && newX < width && newY >= 0 && newY < height)
                    {
                        matriceRotate[x, y] = matrice[newX, newY];
                    }
                    else
                    {
                        // Si les coordonnées sont hors de la matrice, remplir avec un pixel noir
                        matriceRotate[x, y] = new Pixel(0, 0, 0);
                    }
                }
            }

            matrice = matriceRotate;
            return matriceRotate;
        }

        public Pixel[,] Agrandissement(int coefficientAgd)
        {
            int nvlLargeur = matrice.GetLength(0) * coefficientAgd;
            int nvlHauteur = matrice.GetLength(1) * coefficientAgd;
            
            // taille nouvelle matrice = taille de la matrice originale * coefficient d'agrandissement

            Pixel[,] agrandissementMatrice = new Pixel[nvlLargeur, nvlHauteur];

            int originI = 0;
            int originJ = 0;
            for (int i = 0; i < nvlLargeur; i++)
            {
                for (int j = 0; j < nvlHauteur; j++)
                {
                    originI = i / coefficientAgd;
                    originJ = j / coefficientAgd;

                    // verifie que les coord son valides (a l'interieur de la matrice de base)
                    if (originI >= 0 && originI < matrice.GetLength(0) && originJ >= 0 && originJ < matrice.GetLength(1))
                    {
                        agrandissementMatrice[i, j] = matrice[originI, originJ];
                    }

                    // sinon met un pixel noir
                    else
                    {
                        agrandissementMatrice[i, j] = new Pixel(0, 0, 0);
                    }
                }
            }
            
            return agrandissementMatrice;
        }

        public Pixel[,] Convolution(double[,] kernel)
        {
            int width = matrice.GetLength(0);
            int height = matrice.GetLength(1);
            int kernelSize = kernel.GetLength(0);

            // Image de sortie
            Pixel[,] sortieFiltre = new Pixel[width, height];

            // Parcourir tous les pixels de l'image d'entrée
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    // Initialiser la somme des produits
                    double r = 0, g = 0, b = 0;

                    // Parcourir le noyau
                    for (int ki = 0; ki < kernelSize; ki++)
                    {
                        for (int kj = 0; kj < kernelSize; kj++)
                        {
                            // Coordonnées du pixel dans l'image
                            int x = i + ki - kernelSize / 2;
                            int y = j + kj - kernelSize / 2;

                            // Vérifier que le pixel est dans l'image
                            if (x >= 0 && x < width && y >= 0 && y < height)
                            {
                                r += matrice[x, y].R * kernel[ki, kj];
                                g += matrice[x, y].G * kernel[ki, kj];
                                b += matrice[x, y].B * kernel[ki, kj];
                            }
                        }
                    }

                    // Si les valeurs depasse 255 ou sont en dessous de 0
                    sortieFiltre[i, j] = new Pixel(
                        (byte)Math.Min(Math.Max(r, 0), 255),
                        (byte)Math.Min(Math.Max(g, 0), 255),
                        (byte)Math.Min(Math.Max(b, 0), 255)
                        );
                }
            }
            matrice = sortieFiltre;
            return sortieFiltre;
        }


       
        public bool Mandelbrot(double Cr, double Ci, int n)
        {
            double Zr = 0, Zi = 0, ZrNew, ZiNew;
            for (int i = 0; i < n; i++)
            {
                ZrNew = Zr * Zr - Zi * Zi + Cr; // calcule la partie réelle de Z^2 + c 
                ZiNew = 2 * Zr * Zi + Ci; // calcule la partie imaginaire de Z^2 + c 
                Zr = ZrNew;
                Zi = ZiNew;

                // si le module de Z depasse 2 : la suite tends vers l'infini <=> n'appartient pas a l'ensemble
                if (Zr * Zr + Zi * Zi > 4)
                    return false;
            }
            return true;
        }

        public void Fractale(int iteration)
        {

            double cr = 4.5 / hauteurImg;
            double ci = 4.5 / largeurImg;
            bool v;

            // Constantes : Domaine de définition de l'ensenmble de Mandelbrot (voir wikipedia)
            double xMin = -2;
            double xMax = 1;
            double yMin = -1.5;
            double yMax = 1.5;
            double dx = (xMax - xMin) / largeurImg;
            double dy = (yMax - yMin) / hauteurImg;

            for (int i = -hauteurImg / 2; i < hauteurImg / 2; i++)
                for (int j = -largeurImg / 2; j < largeurImg / 2; j++)
                {
                    // calcule les valeurs des coordonnées (x,y) associées à chaque pixel de l'image
                    double x = xMin + (j + largeurImg / 2) * dx;
                    double y = yMin + (i + hauteurImg / 2) * dy;

                    matrice[i + hauteurImg / 2, j + largeurImg / 2] = Mandelbrot(x, y, iteration) ? new Pixel(0, 0, 0) : new Pixel(255, 255, 255);
                }
        }
        
    }
}
