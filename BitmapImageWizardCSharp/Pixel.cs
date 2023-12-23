using System;
namespace ProjetImage
{
    public class Pixel
    {
        byte red;
        byte green;
        byte blue;

        public Pixel(byte red, byte green, byte blue)
        {
            this.red = red;
            this.green = green;
            this.blue = blue;
        }

        public byte R
        {
            get { return red; }
            set { red = value; }
        }
        public byte G
        {
            get { return green; }
            set { green = value; }
        }
        public byte B
        {
            get { return blue; }
            set { blue = value; }
        }

    }
}
