#define DEBUG

using System;
using static Final.Utils;
// ReSharper disable MemberCanBePrivate.Global



namespace Final {
    internal static class Program {
        
        public static readonly string[] Unsearchable = {"PositionHistory", "AvgVelocityHistory"};
        public static readonly string[] AlwaysSmall = {"AvgPos", "Podiums", "First", "Second", "Third"};
        public struct Runner {
            
            public readonly string Name;
            public readonly string Team;

            public readonly double Salary;

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
            for (var i=0; i<state.NumberRunners; i++) {
                state.Runners[i] = new Runner(state.Runners, i,  state.NumberRaces, state.NumberRunners);
            }
        }
        
        private static void SubMenuSorting(Runner[] data)
        {
            var option = AskForOption((1, 4),
                "\nMenu de opciones:\n"+
                "    1. Ordenar por campo\n"+
                "    2. Buscar por campo\n" +
                "    3. Mostrar la tabla\n"+
                "    4. Salir\n"
            );
            
            if (option == 4) return;

            string campo;
            switch (option) {
                case 1:
                    var operadorStr = AskForOption( (1, 3),
                        "\nCómo deseas ordenarlo?\n" +
                        "    1. Mayor a menor\n"+
                        "    2. Menor a mayor\n"+
                        "    3. Cancelar"
                    );
                    
                    if (operadorStr == 3) break;
                    
                    campo = AskForFieldOrProperty<Runner>(
                        prompt:"Cuál es el campo por el que deseas ordenar los datos?: ",
                        except: new []{"PositionHistory", "AvgVelocityHistory", "Name", "Team"}
                    );
                    
                    Operator operador = operadorStr == 1 ? Operator.Biggest : Operator.Smallest;
                
                    Sort(data, campo, operador);
                    break;
                case 2: 
                    // Made with https://stackoverflow.com/questions/16699340/passing-a-type-to-a-generic-method-at-runtime
                    // And https://stackoverflow.com/questions/29978600/c-sharp-can-convert-from-c-sharp-type-to-system-type-but-not-vice-versa
                    campo = AskForFieldOrProperty<Runner>(
                        prompt:"Cuál es el campo por el que deseas filtrar los datos?: ",
                        except: Unsearchable
                    );
                    
                    FilterFieldProperty(data, campo, except: Unsearchable,  small: AlwaysSmall);

                    break;
                case 3:
                    ShowData(data, except: Unsearchable, small: AlwaysSmall);
                    break;

                
            }


        }

        struct AppState {
            public int NumberRaces;
            public int NumberRunners;
            public  Runner[] Runners;
        }

        private static void Main() {

            AppState state = new AppState { Runners = null };
            
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
                        state.NumberRaces = GetInput("Ingresa el número de carreras de la temporada (6-25):",
                            new Tuple<int, int>(6, 25));
                        state.NumberRunners = GetInput("Ingresa el número de corredores en la competencia (6-26): ",
                            new Tuple<int, int>(6, 26));
                        state.Runners = new Runner[state.NumberRunners];
                            
                        InputData(state);
                        break;
                    case 2:
                        if (state.Runners != null) SubMenuSorting(state.Runners);
                        else Console.WriteLine("ERROR: Debes ingresar registros para continuar (en la opción 1)");
                        break;
                }
                
                
                if (option == 3) break;
            }
            
        }
    }
}