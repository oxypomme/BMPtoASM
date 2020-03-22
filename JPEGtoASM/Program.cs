using System;
using System.IO;
using System.Drawing;
using System.Text;
using System.Linq;

namespace JPEGtoASM
{
    internal static class Program
    {
        private static Bitmap Colors = BMPtoASM.Properties.Resources.colors;
        /*private static string[] VGACols =
           {"#FF000000", "#FF0000AA", "#FF00AA00", "#FF00AAAA", "#FFAA0000", "#FFAA00AA", "#FFAA5500", "#FFAAAAAA", "#FF555555", "#FF5555FF",
            "#FF55FF55", "#FF55FFFF", "#FFFF5555", "#FFFF55FF", "#FFFFFF55", "#FFFFFFFF", "#FF000000", "#FF101010", "#FF202020", "#FF353535",
            "#FF454545", "#FF555555", "#FF656565", "#FF757575", "#FF8A8A8A", "#FF9A9A9A", "#FFAAAAAA", "#FFBABABA", "#FFCACACA", "#FFDFDFDF",
            "#FFEFEFEF", "#FFFFFFFF", "#FF0000FF", "#FF4100FF", "#FF8200FF", "#FFBE00FF", "#FFFF00FF", "#FFFF00BE", "#FFFF0082", "#FFFF0041",
            "#FFFF0000", "#FFFF4100", "#FFFF8200", "#FFFFBE00", "#FFFFFF00", "#FFBEFF00", "#FF82FF00", "#FF41FF00", "#FF00FF00", "#FF00FF41",
            "#FF00FF82", "#FF00FFBE", "#FF00FFFF", "#FF00BEFF", "#FF0082FF", "#FF0041FF", "#FF8282FF", "#FF9E82FF", "#FFBE82FF", "#FFDF82FF",
            "#FFFF82FF", "#FFFF82DF", "#FFFF82BE", "#FFFF829E", "#FFFF8282", "#FFFF9E82", "#FFFFBE82", "#FFFFDF82", "#FFFFFF82", "#FFDFFF82",
            "#FFBEFF82", "#FF9EFF82", "#FF82FF82", "#FF82FF9E", "#FF82FFBE", "#FF82FFDF", "#FF82FFFF", "#FF82DFFF", "#FF82BEFF", "#FF829EFF",
            "#FFBABAFF", "#FFCABAFF", "#FFDFBAFF", "#FFEFBAFF", "#FFFFBAFF", "#FFFFBAEF", "#FFFFBADF", "#FFFFBACA", "#FFFFBABA", "#FFFFCABA",
            "#FFFFDFBA", "#FFFFEFBA", "#FFFFFFBA", "#FFEFFFBA", "#FFDFFFBA", "#FFCAFFBA", "#FFBAFFBA", "#FFBAFFCA", "#FFBAFFDF", "#FFBAFFEF",
            "#FFBAFFFF", "#FFBAEFFF", "#FFBADFFF", "#FFBACAFF", "#FF000071", "#FF1C0071", "#FF390071", "#FF550071", "#FF710071", "#FF710055",
            "#FF710039", "#FF71001C", "#FF710000", "#FF711C00", "#FF713900", "#FF715500", "#FF717100", "#FF557100", "#FF397100", "#FF1C7100",
            "#FF007100", "#FF00711C", "#FF007139", "#FF007155", "#FF007171", "#FF005571", "#FF003971", "#FF001C71", "#FF393971", "#FF453971",
            "#FF553971", "#FF613971", "#FF713971", "#FF713961", "#FF713955", "#FF713945", "#FF713939", "#FF714539", "#FF715539", "#FF716139",
            "#FF717139", "#FF617139", "#FF557139", "#FF457139", "#FF397139", "#FF397145", "#FF397155", "#FF397161", "#FF397171", "#FF396171",
            "#FF395571", "#FF394571", "#FF515171", "#FF595171", "#FF615171", "#FF695171", "#FF715171", "#FF715169", "#FF715161", "#FF715159",
            "#FF715151", "#FF715951", "#FF716151", "#FF716951", "#FF717151", "#FF697151", "#FF617151", "#FF597151", "#FF517151", "#FF517159",
            "#FF517161", "#FF517169", "#FF517171", "#FF516971", "#FF516171", "#FF515971", "#FF000041", "#FF100041", "#FF200041", "#FF310041",
            "#FF410041", "#FF410031", "#FF410020", "#FF410010", "#FF410000", "#FF411000", "#FF412000", "#FF413100", "#FF414100", "#FF314100",
            "#FF204100", "#FF104100", "#FF004100", "#FF004110", "#FF004120", "#FF004131", "#FF004141", "#FF003141", "#FF002041", "#FF001041",
            "#FF202041", "#FF282041", "#FF312041", "#FF392041", "#FF412041", "#FF412039", "#FF412031", "#FF412028", "#FF412020", "#FF412820",
            "#FF413120", "#FF413920", "#FF414120", "#FF394120", "#FF314120", "#FF284120", "#FF204120", "#FF204128", "#FF204131", "#FF204139",
            "#FF204141", "#FF203941", "#FF203141", "#FF202841", "#FF2D2D41", "#FF312D41", "#FF352D41", "#FF3D2D41", "#FF412D41", "#FF412D3D",
            "#FF412D35", "#FF412D31", "#FF412D2D", "#FF41312D", "#FF41352D", "#FF413D2D", "#FF41412D", "#FF3D412D", "#FF35412D", "#FF31412D",
            "#FF2D412D", "#FF2D4131", "#FF2D4135", "#FF2D413D", "#FF2D4141", "#FF2D3D41", "#FF2D3541", "#FF2D3141", "#FF000000" };*/

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

                var sb = new StringBuilder();
                sb.Append("nameTMP MACRO xA, yA\n\tpush AX\n\tpush BX\n\n\tmov  AX, xA\n\tmov  BX, yA\n\n");

                try
                {
                    using (var image = new Bitmap(args[0]))
                        for (int x = 0; x < image.Size.Width; x++)
                        {
                            if (x != 0)
                                sb.Append("\n\tinc  AX\n\tmov  BX, yA\n\n");
                            for (int y = 0; y < image.Size.Height; y++)
                            {
                                if (y != 0)
                                    sb.Append("\tinc  BX\n");

                                var pixel = image.GetPixel(x, y);
                                if (!(pixel == Color.Black && Array.Exists(args, s => s.Equals("--t"))))
                                {
                                    sb.Append("\toxgSHOWPIXEL AX, BX, " + GetClosedMatch(pixel));
                                    sb.Append(string.Format("\t; {0}-{1}\n", x, y));
                                }
                            }
                        }
                    try
                    {
                        sb.Append("\n\tpop  BX\n\tpop  AX\nENDM");
                        using (var fileASM = new StreamWriter(new FileStream(name, FileMode.CreateNew, FileAccess.Write)))
                            fileASM.Write(sb.ToString());
                        Console.WriteLine(name + " created");
                    }
                    catch (IOException)
                    {
                        Console.WriteLine("ERROR : Output file already exists");
                    }
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
                Console.WriteLine("Help :\n\tJPEGtoASM <file.bmp> [--t] [--o <output.asm>]");
                Console.WriteLine("--t :\n\tAllow transparency, aka don't draw black pixels");
                Console.WriteLine("--o <file.asm> :\n\tChange the output name");
            }
        }
    }
}