using System;
using System.IO;
using System.Drawing;
using System.Linq;

namespace JPEGtoASM
{
    internal static class Program
    {
        private static Bitmap Colors = BMPtoASM.Properties.Resources.colors;

        public static string GetClosedMatch(Color c)
        {
            int Distance(Color c1, Color c2) => //generate an integer giving the squared euclidian distance between 2 colors
                (c1.R - c2.R) * (c1.R - c2.R) +
                (c1.G - c2.G) * (c1.G - c2.G) +
                (c1.B - c2.B) * (c1.B - c2.B);

            string format(int x, int y) => "0" + y.ToString("X") + x.ToString("X") + "h";

            int closestX = 0, closestY = 0;
            Color closest = Colors.GetPixel(0, 0);
            for (int i = 0; i < Colors.Width; i++)
                for (int j = 0; j < Colors.Height; j++)
                {
                    Color curr = Colors.GetPixel(i, j);
                    if (Distance(c, curr) == 0) // a perfect match
                        return format(i, j);
                    if (Distance(c, closest) > Distance(curr, c))
                    {
                        closestX = i;
                        closestY = j;
                        closest = curr;
                    }
                }
            //we didn't found a perfect match, we give the closest color available
            return format(closestX, closestY);
        }

        private static void Main(string[] args)
        {
            Console.WriteLine("BMP to ASM");
            Console.WriteLine("by OxyTom#1831");
            Console.WriteLine("https://github.com/oxypomme/BMPtoASM");

            Console.WriteLine("\n###\n");

            if (args.Length > 0)
            {
                string name = Path.GetFileNameWithoutExtension(args.First()) + ".asm";
                try
                {
                    if (Array.Exists(args, s => s.Equals("--o")))
                        name = args[Array.IndexOf(args, "--o") + 1];
                }
                catch (IndexOutOfRangeException)
                {
                    Console.WriteLine("ERROR : You must specify an output name");
                    Environment.Exit(0);
                }
                if (File.Exists(name))
                {
                    Console.WriteLine("ERROR : Output file already exists");
                    return;
                }
                var sw = new StreamWriter(name);
                sw.Write("nameTMP MACRO xA, yA\n\tpush AX\n\tpush BX\n\n\tmov  AX, xA\n\tmov  BX, yA\n\n");

                try
                {
                    using (var image = new Bitmap(args[0]))
                        for (int x = 0; x < image.Size.Width; x++)
                        {
                            if (x != 0)
                                sw.Write("\n\tinc  AX\n\tmov  BX, yA\n\n");
                            int offsetSinceLastPixel = 0;
                            for (int y = 0; y < image.Size.Height; y++)
                            {
                                var pixel = image.GetPixel(x, y);
                                if (!(pixel.Equals(Color.FromArgb(255, 0, 0, 0)) && Array.Exists(args, s => s.Equals("--t"))))
                                {
                                    if (offsetSinceLastPixel > 0)
                                        if (offsetSinceLastPixel == 1)
                                            sw.Write("\tinc  BX\n");
                                        else
                                            sw.Write("\tadd  BX, " + offsetSinceLastPixel + "\n");
                                    sw.Write("\toxgSHOWPIXEL AX, BX, " + GetClosedMatch(pixel));
                                    sw.Write(string.Format("\t; {0}-{1}\n", x, y));
                                    offsetSinceLastPixel = 0;
                                }
                                offsetSinceLastPixel++;
                            }
                        }
                    sw.Write("\n\tpop  BX\n\tpop  AX\nENDM");
                    sw.Close();
                    Console.WriteLine(name + " created");
                }
                catch (FileNotFoundException)
                {
                    Console.WriteLine("ERROR : File not found !");
                }
                catch (ArgumentException)
                {
                    Console.WriteLine("ERROR : Error loading file");
                }
            }
            else if (args.Length == 0 || Array.Exists(args, s => s.Equals("help")))
            {
                Console.WriteLine("Help :\n\tBMPtoASM <file.bmp> [--t] [--o <output.asm>]");
                Console.WriteLine("--t [<r> <g> <b>]:\n\tAllow transparency, aka don't draw black pixels by default, or specify another color key (0-255)");
                Console.WriteLine("--o <file.asm> :\n\tChange the output name");
            }
        }
    }
}