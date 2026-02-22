//Programa para leer un audio e imprimir en consola el contenido del audio en texto 
//Para que el programa pueda leer archivos de audio es necesario instalar la biblioteca NAudio y asi leer el contenido del audio
//para transformar el audio a texto se utiliza la biblioteca System.Speech que viene integrada en .NET Framework 
//para instalar NAudio se utiliza el siguiente comando en la consola del administrador de paquetes:
//Install-Package NAudio -Version 2.1.0
//para leer archivos de audio en formato WAV y MP3 se utiliza la biblioteca NAudio
//Autor: Cruz Hernandez William
//Fecha: 15/11/2025
//Esime Culhuacan IPN

using System;

using System.IO;

using System.Linq;

using System.Text;



public class AudioDataProcessor

{

    /// Promedia la matriz de datos de audio por cada milisegundo (1ms).

    /// <param name="Fs">Frecuencia de muestreo (Hz).</param>

    /// <param name="y">Matriz de datos original [muestras, canales].</param>

    /// <returns>Una tupla con (Datos promediados [ms, canales], Vector de tiempo en ms).</returns>

    public (double[,] y_ms, double[] t_ms) GetAveragedDataPerMs(double Fs, double[,] y)

    {

        // Obtener dimensiones

        int numMuestras = y.GetLength(0);

        int numCanales = y.GetLength(1);



        // Número de muestras que corresponden a 1 ms (Equivalente a round(Fs / 1000))

        int samplesPerMs = (int)Math.Round(Fs / 1000.0);



        // Cantidad total de intervalos de 1 ms (Equivalente a floor(numMuestras / samplesPerMs))

        // La división de enteros en C# realiza la función floor.

        int numMs = numMuestras / samplesPerMs;



        // Prealocar matriz para eficiencia: [numMs, numCanales]

        double[,] y_ms = new double[numMs, numCanales];



        // Recorrer cada intervalo de milisegundo

        for (int k = 0; k < numMs; k++) // Índice base 0

        {

            // Definir el rango de muestras para el intervalo actual (base 0)

            int idxIni = k * samplesPerMs;

            int idxFin = (k + 1) * samplesPerMs;



            // Se toma el promedio de amplitud en ese intervalo de 1 ms

            for (int canal = 0; canal < numCanales; canal++)

            {

                double sum = 0.0;



                // Sumar los valores en el intervalo

                for (int i = idxIni; i < idxFin; i++)

                {

                    sum += y[i, canal];

                }



                // Calcular y asignar el promedio

                y_ms[k, canal] = sum / samplesPerMs;

            }

        }



        // Vector de tiempo en milisegundos: [0, 1, 2, ..., numMs-1]

        double[] t_ms = Enumerable.Range(0, numMs).Select(i => (double)i).ToArray();



        // ------------------------------------------------------------

        // IMPRESIÓN DE INFORMACIÓN (Equivalente a fprintf y disp de MATLAB)

        // ------------------------------------------------------------

        Console.WriteLine("\n--- Información de la matriz por cada ms ---");

        Console.WriteLine($"  -> Total de intervalos (ms): {numMs}");

        Console.WriteLine($"  -> Tamaño de la matriz resultante: {numMs} x {numCanales}");



        Console.WriteLine("\n--- Primeros 10 valores (1 ms cada uno) ---");

        int maxRows = Math.Min(10, numMs);

        for (int r = 0; r < maxRows; r++)

        {

            Console.Write($"Ms {r + 1}: ");

            for (int c = 0; c < numCanales; c++)

            {

                // :F6 formatea a 6 decimales.

                Console.Write($"{y_ms[r, c]:F6}\t");

            }

            Console.WriteLine();

        }



        return (y_ms, t_ms);

    }



    // --------------------------------------------------------------------------



    /// <summary>

    /// Guarda la matriz de datos promediados y el vector de tiempo en formato CSV.

    /// Equivalente a la opción '.csv' de la Sección 7 del código MATLAB.

    /// </summary>

    /// <param name="y_ms">Matriz de datos promediados [ms, canales].</param>

    /// <param name="t_ms">Vector de tiempo en ms.</param>

    /// <param name="fullSavePath">Ruta completa del archivo donde guardar.</param>

    public void SaveDataToCsv(double[,] y_ms, double[] t_ms, string fullSavePath)

    {

        // Lógica de guardar en CSV

        try

        {

            var sb = new StringBuilder();

            int numCanales = y_ms.GetLength(1);



            // 1. Crear la fila de encabezado

            sb.Append("Tiempo_ms");

            for (int c = 1; c <= numCanales; c++)

            {

                sb.Append($",Canal_{c}");

            }

            sb.AppendLine();



            // 2. Escribir los datos

            int numMs = y_ms.GetLength(0);

            for (int r = 0; r < numMs; r++)

            {

                // Agregar el valor de tiempo

                sb.Append(t_ms[r]);



                // Agregar los valores de los canales

                for (int c = 0; c < numCanales; c++)

                {

                    // Nota: Se asume coma (,) como separador de columna.

                    sb.Append($",{y_ms[r, c]}");

                }

                sb.AppendLine();

            }



            // 3. Escribir el contenido completo al archivo

            File.WriteAllText(fullSavePath, sb.ToString());



            Console.WriteLine($"\nDatos guardados exitosamente en {fullSavePath} (formato CSV)");

        }

        catch (Exception ex)

        {

            // Manejo de error si no se puede escribir el archivo.

            Console.WriteLine($"\nError al guardar el archivo CSV: {ex.Message}");

        }

    }

}