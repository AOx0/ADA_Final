using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Final {
    public static class Utils {
        /// <summary>
        /// Made with help of https://stackoverflow.com/questions/7613782/iterating-through-struct-members
        /// </summary>
        /// <param name="except"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static string[] GetAllFieldsAndProperties<T>(string[] except = null) {
            var result = new List<string>();
            
            foreach (var field in typeof(T).GetProperties((BindingFlags)(-1) )) { 
                if (except != null && except.Contains(field.Name)) continue;
                result.Add(field.Name);
            }
            
            foreach (var field in typeof(T).GetFields((BindingFlags)(-1) )) { 
                if (except != null && except.Contains(field.Name)) continue;
                result.Add(field.Name);
            }
            
            return result.ToArray();
            
        }
        private static Exception Error => null; // Dummy error, just to have something to throw on some cases
        
        public enum Operator {
            Biggest,
            Smallest
        }
        
        public static int FindElementIndex<T, TU>(T[] data, string campo, TU find,  Tuple<int, int> inRange = null ) {
            for (var i= inRange?.Item1 ?? 0; i< (inRange?.Item2 ?? data.Length); i++) {
                dynamic data1 = data[i].GetType().GetField(campo)?.GetValue(data[i]);
                dynamic data2 = find;

                if (data1?.GetType() != data2.GetType()) continue;
                if (data1 == data2 ) return i;
            }
            
            return -1;
        }
        
        /// <summary>
        /// Function that asks for an input of type T, parses it and returns it.
        /// Made with https://stackoverflow.com/a/12911864/14916353
        /// And with https://stackoverflow.com/a/2961702/14916353
        /// </summary>
        /// <typeparam name="T">Runner type to read</typeparam>
        /// <returns></returns>
        public static T GetInput<T>(string prompt = "> ", Tuple<T, T> inRange = null) where T : IComparable {
            while (true) {
                Console.Write(prompt);
                try {
                    var converter = System.ComponentModel.TypeDescriptor.GetConverter(typeof(T));
                    var input = Console.ReadLine();
                    
                    #if DEBUG
                        Console.WriteLine(input);
                    #endif

                    // If the input of the user is empty
                    // an Error is thrown and the function gets to run again
                    if (input != null && string.IsNullOrEmpty(input.Replace(" ", ""))) throw Error;
                    var result = (T)converter.ConvertFromString(input);

                    if (inRange == null || result == null) return result;
                    if (result.CompareTo(inRange.Item1) >= 0  && result.CompareTo(inRange.Item2) <= 0 ) {
                        return result;
                    }

                    Console.WriteLine($"ERROR: Ingresa un dato en el rango indicado ({inRange.Item1}-{inRange.Item2}).");

                } catch (Exception) { Console.WriteLine("ERROR: Ingresa un dato válido."); }
            }
        }

        /// <summary>
        /// A function that asks for an option from a Menu that is displayed with custom range of options.
        /// The function asks for the input undefined times until the user writes a valid option.
        /// </summary>
        /// <param name="inRange">A tuple with (minOption, maxOption)</param>
        /// <param name="menu">String to be displayed to inform the user about the available options</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">Exception thrown when the programmer calls the function with invalid min-max option values</exception>
        public static int AskForOption(Tuple<int, int> inRange, string menu) {
            int opcion;

            if (inRange.Item1 < 0)  throw new ArgumentException("Opción minima debe ser mayor a 0");
            if (inRange.Item1 == inRange.Item2)  throw new ArgumentException("Las opciones deben ser distintas");
            
            Console.WriteLine(menu);
            do {
                opcion = GetInput($"Ingresa una opción ({inRange.Item1}-{inRange.Item2}): ", inRange);
                if (opcion <= inRange.Item2 && opcion >= inRange.Item1) break;
            } while (true);

            return opcion;

        }

        /// <summary>
        /// Made with https://stackoverflow.com/questions/7613782/iterating-through-struct-members
        /// </summary>
        /// <param name="data"></param>
        /// <param name="inRange"></param>
        /// <param name="except"></param>
        /// <param name="small"></param>
        /// <typeparam name="T"></typeparam>
        public static void ShowData<T>(T[] data, Tuple<int, int> inRange = null, string[] except = null, string[] small = null) {
            
            
            
            foreach (var field in typeof(T).GetProperties((BindingFlags)(-1) )) { 
                if (except != null && except.Contains(field.Name)) continue;
                Console.Write("{0,-5}  ", field.Name);
            }
            
            foreach (var field in typeof(T).GetFields((BindingFlags)(-1) )) { 
                if (except != null && except.Contains(field.Name)) continue;
                Console.Write(small != null && small.Contains(field.Name) ? "{0,-10}" :  "{0,-20}  ", field.Name);
            }
            
            Console.Write("\n");

            for (var i= inRange?.Item1 ?? 0; i< (inRange?.Item2 ?? data.Length); i++) {
                foreach (var field in typeof(T).GetProperties((BindingFlags)(-1) )) {
                    if (except != null && except.Contains(field.Name)) continue;
                    Console.Write("{0,-5}  ", field.GetValue(data[i])?.ToString()?.Replace("00:00:00", ""));
                } 
                foreach (var field in typeof(T).GetFields((BindingFlags)(-1) )) {
                    if (except != null && except.Contains(field.Name)) continue;
                    Console.Write(small != null && small.Contains(field.Name) ? "{0,-10}" :  "{0,-20}  ", 
                        field.GetValue(data[i])?.ToString()?.Replace("00:00:00", ""));
                } 
                Console.Write("\n");
            }

            
        }
        
        public static void Sort<T>(T[] data, string campo, Operator withOperator, Tuple<int, int> inRange = null) {
            bool resultado = false;

            for (int j = 0; j < data.Length - 1; j++)
            {
                for (int i = 0; i < data.Length - 1 - j; i++)
                {
                    dynamic data1 = data[i].GetType().GetProperty(campo);
                    dynamic data2 = data[i+1].GetType().GetProperty(campo);

                    resultado = withOperator switch {
                        Operator.Biggest => data2 > data1,
                        Operator.Smallest => data2 < data1,
                        _ => resultado
                    };

                    if (!resultado) continue;
                    
                    (data[i], data[i + 1]) = (data[i + 1], data[i]);
                }
            }
            ShowData(data, inRange);
        }
    }
}