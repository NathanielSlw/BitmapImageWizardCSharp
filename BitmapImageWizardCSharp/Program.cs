using System;

namespace ProjetImage
{
    class Program
    {
        static void Main(string[] args)
        {
          




            double[,] filtreDetectionDeContours = new double[,] {
                {-1, -1, -1},
                {-1, 8, -1},
                {-1, -1, -1}
            };

            double[,] renforcement = new double[,]
            {
                { 0, -1, 0 },
                { -1, 5, -1 },
                { 0, -1, 0 }
            };

            double[,] flou = {
                {0.004, 0.016, 0.026, 0.016, 0.004},
                {0.016, 0.071, 0.117, 0.071, 0.016},
                {0.026, 0.117, 0.000, 0.117, 0.026},
                {0.016, 0.071, 0.117, 0.071, 0.016},
                {0.004, 0.016, 0.026, 0.016, 0.004}
            };

           

            Console.WriteLine("Veuillez choisir une image (lac, coco, Test)");
            string nom = Console.ReadLine();
            while (nom == null || (nom != "lac" && nom != "coco" && nom != "Test"))
            {
                Console.WriteLine(nom + " n'existe pas");
                Console.WriteLine("Veuillez choisir une image (lac, coco, Test)");
                nom = Console.ReadLine();
            }
            string nomFile = "./Images/" + nom + ".bmp";
            Image image = new Image(nomFile);
            Console.Clear();
            Console.WriteLine("Image sélectionné : " + nom);

            Console.WriteLine("Veuillez choisir un nombre entre 1 et 7");
            Console.WriteLine("1 : Mettre en gris \n2 : Agrandir\n3 : Rotation \n4 : Flou\n5 : Netteté\n6 : Detection des contours\n7 : Fractale Mandelbrot");
            int choix;
            choix = Convert.ToInt32(Console.ReadLine());

            Console.Clear();
            switch (choix)
            {
                case 1:
                    Console.WriteLine("Image mise en grise (Voir dossier Images : Sortie.bmp)");
                    image.MettreEnGris();
                    break;
                case 2:
                    Console.WriteLine("Agrandir");
                    Console.WriteLine("Veuillez choisir une taille d'agrandissement ");
                    try
                    {
                        int agd = Convert.ToInt32(Console.ReadLine());
                        image = new Image(nomFile, agd);
                        Console.WriteLine("Image agrandit (Voir dossier Images : Sortie.bmp)");
                    }
                    catch { Console.WriteLine("Erreur : Veuillez entrer un nombre valide"); }
                    break;
                case 3:
                    Console.WriteLine("Rotation");
                    Console.WriteLine("Veuillez choisir un angle de rotation ");
                    try
                    {
                        int angle = Convert.ToInt32(Console.ReadLine());
                        image.Rotate(angle);
                        Console.WriteLine("L'image a bien subit une rotation (Voir dossier Images : Sortie.bmp)");
                    }
                    catch
                    {
                        Console.WriteLine("Erreur : Veuillez entrer un angle valide");
                    }
                    
                    break;
                case 4:
                    Console.WriteLine("Flou");
                    image.Convolution(flou);
                    Console.WriteLine("L'image a été flouté (Voir dossier Images : Sortie.bmp)");
                    break;
                case 5:
                    Console.WriteLine("Netteté");
                    image.Convolution(renforcement);
                    Console.WriteLine("Netteté effectué (Voir dossier Images : Sortie.bmp)");
                    break;
                case 6:
                    Console.WriteLine("Detection des contours");
                    image.Convolution(filtreDetectionDeContours);
                    Console.WriteLine("Detection de contours effectué (Voir dossier Images : Sortie.bmp)");
                    break;
                case 7:
                    Console.WriteLine(" Fractale Mandelbrot");
                    Console.WriteLine("Veuillez choisir les dimensions de la fractale (hauteur,largeur)");
                    try
                    {
                        int hauteur = Convert.ToInt32(Console.ReadLine());
                        int largeur = Convert.ToInt32(Console.ReadLine());
                        Console.Clear();
                        Console.WriteLine("Vous avez choisi une hauteur de : " + hauteur + " et une largeur de " + largeur);
                        image = new Image(hauteur, largeur);
                        Console.WriteLine("Veuillez choisir un nombre d'itération pour la fractale de Mandelbrot (Essaye 10, 15, 100, 1000)");
                        int iteration = Convert.ToInt32(Console.ReadLine());
                        image.Fractale(iteration);
                        Console.WriteLine("Fractale effectué (Voir dossier Images : Sortie.bmp)");
                    }
                    catch 
                    {
                        Console.WriteLine("Erreur : Veuillez entrez des nombres valides");
                    }
                    break;
                default:
                    Console.WriteLine("Entree invalide");
                    break;
            }
            image.FromImageToFile(nomFile);

            Console.ReadKey();
        }
    }
}

