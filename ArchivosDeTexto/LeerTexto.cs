//Programa en c# para leer un archivo de texto, mostrar su contenido y calcular su entropía.
//para leer archivos de texto se necesita la libreria System.IO
//Creado por: William Cruz Hernandez
//Fecha: 15/09/2025
//Esime Culhuacan IPN

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq; // Necesario para Count y Aggregate

namespace ArchivosDeTexto
{
    internal class LeerTexto
    {
        // Función auxiliar para calcular el logaritmo base 2
        private static double Log2(double value)
        {
            // Retorna 0 si el valor es 0 (para evitar log(0) en el cálculo de entropía)
            if (value <= 0) return 0;
            return Math.Log(value) / Math.Log(2.0);
        }

        private static void CalcularYMostrarEntropia(string contenido)
        {
            if (string.IsNullOrEmpty(contenido))
            {
                Console.WriteLine("El archivo está vacío, la entropía es 0.");
                return;
            }

            // 1. Conteo de frecuencias (F_i)
            // Se usa el caracter como símbolo (incluyendo espacios, saltos de línea, etc.)
            var frecuencias = contenido
                .GroupBy(c => c)
                .Select(g => new
                {
                    Simbolo = g.Key,
                    Frecuencia = g.Count()
                })
                .OrderByDescending(x => x.Frecuencia)
                .ToList();

            // Número total de símbolos (N)
            long totalSimbolos = contenido.Length;
            double entropiaTotal = 0.0;

            // 2. Mostrar la tabla de resultados
            Console.WriteLine("\n==========================================================================");
            Console.WriteLine($"| {"Símbolo (x_i)",-15} | {"Frecuencia (F_i)",-16} | {"Probabilidad p(x_i)",-18} | {"p(x_i) log2(1/p(x_i))",-25} |");
            Console.WriteLine("==========================================================================");

            foreach (var item in frecuencias)
            {
                double probabilidad = (double)item.Frecuencia / totalSimbolos;
                // Calculamos el término de entropía: p(x_i) * log2(1/p(x_i))
                double terminoEntropia = probabilidad * Log2(1.0 / probabilidad);
                entropiaTotal += terminoEntropia;

                // Formateamos el símbolo para que sea visible (ej. ' ' -> [Espacio], '\n' -> [NL])
                string simboloDisplay;
                if (item.Simbolo == ' ') simboloDisplay = "[Espacio]";
                else if (item.Simbolo == '\n') simboloDisplay = "[NL]"; // New Line
                else if (item.Simbolo == '\r') simboloDisplay = "[CR]"; // Carriage Return
                else if (item.Simbolo == '\t') simboloDisplay = "[Tab]";
                else simboloDisplay = item.Simbolo.ToString();

                Console.WriteLine($"| {simboloDisplay,-15} | {item.Frecuencia,-16} | {probabilidad,-18:F8} | {terminoEntropia,-25:F8} |");
            }

            Console.WriteLine("==========================================================================");
            // Mostrar los totales
            Console.WriteLine($"| {"Número total de símbolos:",-15} | {totalSimbolos,-16} | {"",-18} | {"Entropía (H):",-25} |");
            Console.WriteLine($"| {"",-15} | {"",-16} | {"",-18} | {entropiaTotal,-25:F8} |");
            Console.WriteLine("==========================================================================");
        }

        static void Main(string[] args)
        {
            //Iniciamos un ciclo para que el programa se ejecute hasta que el usuario decida salir
            while (true)
            {
                try
                {
                    //Pedimos al usuario la ruta del archivo de texto
                    Console.WriteLine("Ingrese la ruta del archivo de texto o 'salir' para terminar:");
                    string rutaArchivo = Console.ReadLine();

                    //Si el usuario ingresa 'salir' terminamos el programa
                    if (rutaArchivo.ToLower() == "salir")
                    {
                        break;
                    }

                    //Leemos el archivo de texto
                    string contenido = File.ReadAllText(rutaArchivo);

                    // Mostramos el contenido (solo las primeras 10 líneas para no saturar la consola)
                    Console.WriteLine("\n--- Contenido del archivo (primeras 10 líneas) ---");
                    string[] lineas = contenido.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
                    for (int i = 0; i < Math.Min(10, lineas.Length); i++)
                    {
                        Console.WriteLine(lineas[i]);
                    }
                    if (lineas.Length > 10) Console.WriteLine("[... Contenido truncado para visualización ...]");
                    Console.WriteLine("--- Fin del contenido ---\n");

                    // Calculamos y mostramos la entropía
                    CalcularYMostrarEntropia(contenido);

                    Console.WriteLine("\nArchivo leído y entropía calculada correctamente.");
                    Console.WriteLine(); //Línea en blanco para separar lecturas
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Ocurrió un error al leer el archivo o calcular la entropía: " + ex.Message);
                }
            }
        }
    }
}