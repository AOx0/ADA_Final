#define DEBUG

using System;
using System.Linq;
using static Final.Utils;
// ReSharper disable MemberCanBePrivate.Global


namespace Final {
    internal static class Program {
        public struct Runner {
            
            public string Name;
            public string Team;

            public double Salary;

            public readonly DateTime Birthday;

            
            public readonly int[] PositionHistory;
            public readonly float[] AvgVelocityHistory;

            public int Age {
                get {
                    var today = DateTime.Today;
                    var age = today.Year - Birthday.Year;
                    if (Birthday.Date > today.AddYears(-age)) age--;
                    return age;
                }
            }
            
            public float AvgSpeed;
            public float AvgPos;
            public int Podiums;
            public int First;
            public int Second;
            public int Third;
            
            private int NumberOfMatches(int pos) {
                int sum = 0;
                foreach (var position in PositionHistory) {
                    if (pos == position) sum++;
                }
                
                return sum;
            }

            public double TotalSalary() {
                int result = 0;
                return result;
            }
            
            
            public Runner(Runner[] alreadyRegistered, int numRegistered, int numberRaces, int numberRunners) {
                while (true) {
                    Name = GetInput<string>($"\nIngresa el nombre del corredor {numRegistered + 1}: ");
                    if (FindElementIndex(alreadyRegistered, "Name", Name , new Tuple<int, int>(0, numRegistered)) == -1) break;
                    Console.WriteLine("ERROR: Un corredor con ese nombre ya está registrado");
                }
               
                Team = GetInput<string>("Ingresa el nombre del Team: ");
                Salary =  GetInput<double>("Ingresa el salario base: $");
                while (true) {
                    try {
                        Birthday = DateTime.ParseExact(GetInput<string>("Ingresa la fecha de nacimiento (dd/MM/yyyy): "), "dd/MM/yyyy", null);
                        break;
                    } catch (Exception) {
                        Console.WriteLine("ERROR: Ingresa el formato correcto dd/MM/yyyy");
                    }
                }
                
                PositionHistory = new int[numberRaces];
                AvgVelocityHistory = new float[numberRaces];
                
                AvgPos = 0;
                Podiums = 0;
                First = 0;
                Second = 0;
                Third = 0;
                AvgSpeed = 0;

                for (var raceN=0; raceN < numberRaces; raceN++) {
                    
                    PositionHistory[raceN] = GetInput($"Ingresa la posición del corredor en la carrera {raceN} (1-{numberRunners}): ",
                        new Tuple<int, int>(1, numberRunners));

                    bool wasRegistered = false;
                    for (var pilotN=0; pilotN< numRegistered; pilotN++) {
                        if (alreadyRegistered[pilotN].PositionHistory[raceN] != PositionHistory[raceN]) continue;
                        Console.WriteLine($"ERROR: Otro corredor ya fue registrado con esa posición en la carrera {raceN}");
                        raceN--;
                        wasRegistered = true;
                        break;
                    }
                    
                    if (wasRegistered) continue;
                    
                    AvgVelocityHistory[raceN] =
                        GetInput($"Ingresa la velocidad media del corredor en la carrera {raceN} (1-500): ",
                            new Tuple<float, float>(0, 500));
                    
                    AvgPos += PositionHistory[raceN];
                    AvgSpeed += AvgVelocityHistory[raceN];
                    
                    if (PositionHistory[raceN] >= 1 && PositionHistory[raceN] <= 3) Podiums++;

                    switch (PositionHistory[raceN]) {
                        case 1: First++; break;
                        case 2: Second++; break;
                        case 3: Third++; break;
                    }
                }
                AvgPos /= numberRaces;
                AvgSpeed /= numberRaces;
            }
        }

        private static void InputData(AppState state) {
            for (var i=0; i<state.numberRunners; i++) {
                state.runners[i] = new Runner(state.runners, i,  state.numberRaces, state.numberRunners);
            }
        }
        
        private static void SubMenu(Runner[] data)
        {
            while (true)
            {
                var optionGral = AskForOption((1, 5),
                    "\nMenu de opciones:\n"+
                    "    1. Ordenar por campo\n"+
                    "    2. Buscar por ID\n"+
                    "    3. Mostrar resumen\n"+
                    "    4. Mostrar 4 propiedades con mejor promedio\n"+
                    "    5. Salir"
                );
                
                if (optionGral == 5) break;
                
                switch (optionGral)
                {
                    case 1:
                        
                        break;
                    case 2:
                        //SubMenuSearch(data);
                        break;
                    case 3:
                        ShowNutshell(data);
                        break;
                    case 4:
                        Sort(data, "RunnerField.DonationsAvg", Operator.Biggest,
                            new Tuple<int, int>(0, 4)
                        );
                        break;
                }
            }
        }
        
        private static void SubMenuSorting(Runner[] data)
        {
            var option = AskForOption((1, 4),
                "\nMenu de opciones ordenamiento:\n"+
                "    1. Ordenar por campo\n"+
                "    2. Buscar por campo\n" +
                "    3. Mostrar la tabla\n"+
                "    4. Salir\n"
            );
            
            if (option == 4) return; 

            switch (option) {
                case 1:
                    var operadorStr = AskForOption( (1, 3),
                        "\nCómo deseas ordenarlo?\n" +
                        "    1. Mayor a menor\n"+
                        "    2. Menor a mayor\n"+
                        "    3. Cancelar"
                    );
                    
                    if (operadorStr == 3) break;
                    
                    var campos = GetAllFieldsAndProperties<Runner>(except: new []{"PositionHistory", "AvgVelocityHistory", "Name", "Team"});
                    Console.Write("Campos disponibles: "); foreach (var c in campos) Console.Write(c + " ");  Console.Write("\n");
                
                    string campo;
                    
                    while (true) {
                        campo = GetInput<string>(prompt: "¿Cuál es el campo por el que deseas ordenar los datos?: ");
                        if (campos.Contains(campo)) break;
                        Console.WriteLine("ERROR: El campo ingresado no es válido");
                    }
                    
                    Operator operador = operadorStr == 1 ? Operator.Biggest : Operator.Smallest;
                
                    Sort(data, campo, operador);
                    break;
                case 3:
                    ShowData(data, except: new []{"PositionHistory", "AvgVelocityHistory"}, 
                        small: new []{"AvgPos", "Podiums", "First", "Second", "Third"});
                    break;

                
            }


        }

        private static void SubMenuSearch(Runner[] data, string campo, string  id) {
            
            var indexId = FindElementIndex(data, campo, id);
            if (indexId == -1) {
                Console.WriteLine("ERROR: El valor no existe\n"); return;
            } 
            ShowData(data, new Tuple<int, int>(indexId, indexId+1));
        }
        
        private static void ShowNutshell(Runner[] data)  {
            
            
            foreach (var info in data) {
                
                //promedioPisos += info.NumOfFloors;
            }
        }
        
        struct AppState {
            public int numberRaces;
            public int numberRunners;
            public  Runner[] runners;
        }

        private static void Main() {
            /* 
            Console.WriteLine(string.Join(", ", GetAllFieldsAndProperties<Runner>(
                except: new []{"PositionHistory", "AvgVelocityHistory"}
            )));
            */
            
            AppState state = new AppState {
                runners = null
            };


            while (true) {

                var option = AskForOption((1, 3),
                    "\nMenu de opciones:\n"+
                    "    1. Entrada de datos\n"+
                    "    2. Resultados\n" +
                    "    3. Salir"
                );

                switch (option)
                {
                    case 1:
                        state.numberRaces = GetInput("Ingresa el número de carreras de la temporada (6-25):",
                            new Tuple<int, int>(6, 25));
                        state.numberRunners = GetInput("Ingresa el número de corredores en la competencia (6-26): ",
                            new Tuple<int, int>(6, 26));
                        state.runners = new Runner[state.numberRunners];
                            
                        InputData(state);
                        break;
                    case 2:
                        if (state.runners != null)
                        {
                            SubMenuSorting(state.runners);
                        }
                        else Console.WriteLine("ERROR: Debes ingresar registros para continuar (en la opción 1)");
                        break;
                }
                
                
                if (option == 3) break;
            }
            
        }
    }
}