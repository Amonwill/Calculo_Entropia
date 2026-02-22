//Programa para leer una imagen, identificar si esta es a color o en escala de grises, 
//posteriormente mostrar en consola sus valores RGB o de escala de grises en arreglos correspondientes,
//y calcular la entropía para todos los canales (R, G, B) si es a color o solo R (gris).
//Finalmente guardar los valores en un archivo de texto .txt
//para leer imagenes se necesita la libreria System.Drawing
//Autor: Cruz Hernandez William
//Fecha: 15/11/2025
//Esime Culhuacan IPN

using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace LecturadeImagen
{
    internal class Lectura
    {
        // Función auxiliar para calcular el logaritmo base 2
        private static double Log2(double value)
        {
            // Retorna 0 si el valor es 0 (para evitar log(0) en el cálculo de entropía)
            if (value <= 0) return 0;
            return Math.Log(value) / Math.Log(2.0);
        }

        private static double CalcularYMostrarEntropiaDeCanal(List<int> valoresCanal, string nombreCanal)
        {
            if (!valoresCanal.Any())
            {
                Console.WriteLine($"No hay datos en el canal {nombreCanal}. Entropía: 0.0");
                return 0.0;
            }

            // 1. Conteo de frecuencias (F_i) para niveles de canal (0-255)
            // Se usa cada valor de 0 a 255 como un símbolo discreto (x_i)
            var frecuencias = valoresCanal
                .GroupBy(g => g)
                .Select(g => new
                {
                    Simbolo = g.Key,
                    Frecuencia = g.Count()
                })
                .OrderBy(x => x.Simbolo)
                .ToList();

            // Número total de símbolos (N)
            long totalSimbolos = valoresCanal.Count;
            double entropiaTotal = 0.0;

            // 2. Mostrar la tabla de resultados
            Console.WriteLine($"\n==========================================================================");
            Console.WriteLine($"Calculando Entropía para el Canal: {nombreCanal}");
            Console.WriteLine($"==========================================================================");
            Console.WriteLine($"| {"Símbolo (x_i)",-15} | {"Frecuencia (F_i)",-16} | {"Probabilidad p(x_i)",-18} | {"p(x_i) log2(1/p(x_i))",-25} |");
            Console.WriteLine($"==========================================================================");

            foreach (var item in frecuencias)
            {
                double probabilidad = (double)item.Frecuencia / totalSimbolos;
                // Calculamos el término de entropía: p(x_i) * log2(1/p(x_i))
                double terminoEntropia = probabilidad * Log2(1.0 / probabilidad);
                entropiaTotal += terminoEntropia;

                // Solo mostramos las entradas que tienen frecuencia > 0 para no saturar la consola
                if (item.Frecuencia > 0)
                {
                    Console.WriteLine($"| {item.Simbolo,-15} | {item.Frecuencia,-16} | {probabilidad,-18:F8} | {terminoEntropia,-25:F8} |");
                }
            }

            Console.WriteLine($"==========================================================================");
            // Mostrar los totales
            Console.WriteLine($"| {"Número total de símbolos:",-15} | {totalSimbolos,-16} | {"",-18} | {"Entropía (H):",-25} |");
            Console.WriteLine($"| {"",-15} | {"",-16} | {"",-18} | {entropiaTotal,-25:F8} |");
            Console.WriteLine($"==========================================================================");

            return entropiaTotal;
        }

        static void Main(string[] args)
        {
            try
            {
                //Primero pedimos al usuario la ruta de la imagen a leer
                Console.WriteLine("Ingrese la ruta de la imagen a leer (incluya la extension, por ejemplo .jpg o .png):");
                string rutaImagen = Console.ReadLine();

                using (Bitmap imagen = new Bitmap(rutaImagen)) //Se carga la imagen en un objeto Bitmap
                {
                    bool esColor = false; //Variable para identificar si la imagen es a color o en escala de grises

                    // Listas para almacenar los valores de cada canal
                    List<int> canalR = new List<int>();
                    List<int> canalG = new List<int>();
                    List<int> canalB = new List<int>();

                    // 1. Detección de color y recolección de valores de los 3 canales
                    for (int y = 0; y < imagen.Height; y++) //Se recorre cada pixel de la imagen con respecto a su altura
                    {
                        for (int x = 0; x < imagen.Width; x++) //Se recorre cada pixel de la imagen con respecto a su anchura
                        {
                            Color pixel = imagen.GetPixel(x, y); //Se obtiene el color del pixel en la posicion (x,y)

                            // Guardamos los valores de los tres canales
                            canalR.Add(pixel.R);
                            canalG.Add(pixel.G);
                            canalB.Add(pixel.B);

                            if (pixel.R != pixel.G || pixel.R != pixel.B || pixel.G != pixel.B) //Si los valores de R, G y B son diferentes, la imagen es a color
                            {
                                esColor = true; //Se actualiza la variable esColor a true
                            }
                        }
                    }

                    // 2. Impresión de resultados generales
                    Console.WriteLine("\nDensidad de pixeles de la imagen: ");
                    Console.WriteLine($"{imagen.Width} x {imagen.Height}");
                    Console.WriteLine(esColor ? "La imagen es a color." : "La imagen es en escala de grises.");

                    // 3. Calculamos la entropía

                    // Siempre calculamos el canal R (o Grayscale)
                    double entropiaR = CalcularYMostrarEntropiaDeCanal(canalR, "Rojo (R / Grayscale)");
                    double entropiaG = 0.0;
                    double entropiaB = 0.0;

                    if (esColor)
                    {
                        // Si es a color, calculamos también G y B
                        entropiaG = CalcularYMostrarEntropiaDeCanal(canalG, "Verde (G)");
                        entropiaB = CalcularYMostrarEntropiaDeCanal(canalB, "Azul (B)");

                        double entropiaPromedio = (entropiaR + entropiaG + entropiaB) / 3.0;

                        Console.WriteLine("\n==========================================================================");
                        Console.WriteLine($"RESUMEN DE ENTROPÍA (IMAGEN A COLOR)");
                        Console.WriteLine($"Entropía Canal R: {entropiaR:F8}");
                        Console.WriteLine($"Entropía Canal G: {entropiaG:F8}");
                        Console.WriteLine($"Entropía Canal B: {entropiaB:F8}");
                        Console.WriteLine($"Entropía Promedio (Referencia): {entropiaPromedio:F8} bits/símbolo");
                        Console.WriteLine("==========================================================================");
                    }
                    else
                    {
                        Console.WriteLine("\n==========================================================================");
                        Console.WriteLine($"RESUMEN DE ENTROPÍA (ESCALA DE GRISES)");
                        Console.WriteLine($"Entropía Total (Canal R): {entropiaR:F8} bits/símbolo");
                        Console.WriteLine("==========================================================================");
                    }


                    // 4. Impresión de valores (se mantiene el código original de impresión)
                    Console.WriteLine("\n--- Valores de píxeles (parcial) ---");
                    for (int y = 0; y < Math.Min(10, imagen.Height); y++)
                    {
                        for (int x = 0; x < Math.Min(20, imagen.Width); x++)
                        {
                            Color pixel = imagen.GetPixel(x, y);
                            if (esColor)
                            {
                                Console.Write($"({pixel.R},{pixel.G},{pixel.B}) ");
                            }
                            else
                            {
                                Console.Write($"{pixel.R} "); // En escala de grises, R, G y B son iguales
                            }
                        }
                        if (imagen.Width > 20) Console.Write("...");
                        Console.WriteLine();
                    }
                    if (imagen.Height > 10) Console.WriteLine("[... Más filas de píxeles ...]");
                    Console.WriteLine("--- Fin de la impresión parcial ---\n");


                    // 5. Guardado en archivo de texto
                    Console.WriteLine("Ingrese la ruta donde desea guardar el archivo de texto (incluya la extension .txt):");
                    string rutaArchivo = Console.ReadLine();
                    using (StreamWriter archivo = new StreamWriter(rutaArchivo))
                    {
                        archivo.WriteLine($"{imagen.Width} {imagen.Height}");
                        archivo.WriteLine(imagen.PixelFormat);
                        archivo.WriteLine(esColor ? "La imagen es a color." : "La imagen es en escala de grises.");

                        for (int y = 0; y < imagen.Height; y++)
                        {
                            for (int x = 0; x < imagen.Width; x++)
                            {
                                Color pixel = imagen.GetPixel(x, y);
                                if (esColor)
                                {
                                    archivo.Write($"{pixel.R} {pixel.G} {pixel.B} ");
                                }
                                else
                                {
                                    archivo.Write($"{pixel.R} "); // En escala de grises, R, G y B son iguales
                                }
                            }
                            archivo.WriteLine();
                        }
                    }
                    Console.WriteLine($"Archivo de valores de píxeles guardado en: {rutaArchivo}");
                }
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("Error: El archivo de imagen no se encontró en la ruta especificada.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ocurrió un error: {ex.Message}");
            }

            //Finalmente, se espera a que el usuario presione una tecla para finalizar el programa
            Console.WriteLine("Presione una tecla para finalizar el programa.");
            Console.ReadLine();
        }
    }
}