using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using NAudio.wave;

namespace HuffmanAudioIPN
{
    // 1. Estructura para el Árbol de Huffman
    public class NodoHuffman
    {
        public double Simbolo { get; set; }
        public int Frecuencia { get; set; }
        public NodoHuffman Izquierda { get; set; }
        public NodoHuffman Derecha { get; set; }

        public bool EsHoja => Izquierda == null && Derecha == null;
    }

    class Program
    {
        // Ruta de archivo MP3
        const string RUTA_AUDIO = @"E:\William\Universidad\semestre 7\Teoria de la informacion y codificacion\Evaluacion\Metro Boomin - Am I Dreaming.mp3";

        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            try
            {
                if (!File.Exists(RUTA_AUDIO))
                {
                    Console.WriteLine("Error: No se encuentra el archivo en la ruta especificada.");
                    return;
                }

                Console.WriteLine(">>> Iniciando Procesamiento de Huffman para: Metro Boomin - Am I Dreaming");

                // Leer Audio y Quantizar para crear símbolos finitos
                var (muestras, fs) = LeerYQuantizarAudio(RUTA_AUDIO);

                // Contar Frecuencias
                var diccFrecuencias = muestras
                    .GroupBy(x => x)
                    .ToDictionary(g => g.Key, g => g.Count());

                // Construir Árbol de Huffman
                Console.WriteLine("\n[1] Construyendo Árbol de Huffman...");
                NodoHuffman raiz = ConstruirArbol(diccFrecuencias);

                // Generar Bits de Codificación
                var tablaCodigos = new Dictionary<double, string>();
                GenerarCodigos(raiz, "", tablaCodigos);

                // Mostrar Resultados 
                MostrarResultados(diccFrecuencias, tablaCodigos, muestras.Length);

                Console.WriteLine("\n[2] Representación del Árbol (Primeros niveles):");
                ImprimirArbol(raiz, "", true);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
            Console.WriteLine("\nPresione cualquier tecla para salir...");
            Console.ReadKey();
        }

        static (double[] muestras, float fs) LeerYQuantizarAudio(string ruta)
        {
            using (var reader = new AudioFileReader(ruta))
            {
                float[] buffer = new float[reader.Length];
                int totalLeido = reader.Read(buffer, 0, (int)reader.Length);
                double[] quantizadas = buffer.Take(totalLeido).Select(x => Math.Round((double)x, 2)).ToArray();
                return (quantizadas, reader.WaveFormat.SampleRate);
            }
        }

        static NodoHuffman ConstruirArbol(Dictionary<double, int> frecuencias)
        {
            var prioridad = new List<NodoHuffman>();
            foreach (var kvp in frecuencias)
                prioridad.Add(new NodoHuffman { Simbolo = kvp.Key, Frecuencia = kvp.Value });

            while (prioridad.Count > 1)
            {
                prioridad = prioridad.OrderBy(x => x.Frecuencia).ToList();

                var izq = prioridad[0];
                var der = prioridad[1];

                var padre = new NodoHuffman
                {
                    Frecuencia = izq.Frecuencia + der.Frecuencia,
                    Izquierda = izq,
                    Derecha = der
                };

                prioridad.RemoveRange(0, 2);
                prioridad.Add(padre);
            }
            return prioridad.First();
        }

        static void GenerarCodigos(NodoHuffman nodo, string codigo, Dictionary<double, string> tabla)
        {
            if (nodo == null) return;
            if (nodo.EsHoja) tabla[nodo.Simbolo] = codigo;

            GenerarCodigos(nodo.Izquierda, codigo + "0", tabla);
            GenerarCodigos(nodo.Derecha, codigo + "1", tabla);
        }

        static void MostrarResultados(Dictionary<double, int> frec, Dictionary<double, string> codigos, int totalN)
        {
            Console.WriteLine("\n=============================================================================");
            Console.WriteLine(string.Format("| {0,-12} | {1,-12} | {2,-15} | {3,-15} |", "Amplitud(xi)", "Freq (Fi)", "Prob p(xi)", "Bits Huffman"));
            Console.WriteLine("=============================================================================");

            foreach (var item in frec.OrderByDescending(x => x.Value).Take(20)) // Top 20 símbolos
            {
                double p = (double)item.Value / totalN;
                Console.WriteLine(string.Format("| {0,12:F2} | {1,12} | {2,15:F6} | {3,-15} |",
                    item.Key, item.Value, p, codigos[item.Key]));
            }
            Console.WriteLine("=============================================================================");
            Console.WriteLine($"Total de muestras analizadas (N): {totalN}");
        }

        static void ImprimirArbol(NodoHuffman nodo, string ident, bool ultimo)
        {
            if (nodo == null) return;

            Console.Write(ident);
            if (ultimo) { Console.Write("└── "); ident += "    "; }
            else { Console.Write("├── "); ident += "│   "; }

            if (nodo.EsHoja) Console.WriteLine($"[Símbolo: {nodo.Simbolo} Freq: {nodo.Frecuencia}]");
            else Console.WriteLine($"[Suma Freq: {nodo.Frecuencia}]");

            // Solo imprimimos una rama pequeña para no saturar la consola
            if (nodo.Frecuencia > (nodo.Frecuencia * 0.1))
            {
                ImprimirArbol(nodo.Izquierda, ident, false);
                ImprimirArbol(nodo.Derecha, ident, true);
            }
        }
    }
}