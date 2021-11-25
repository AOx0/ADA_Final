using System;
using static Final.Utils;
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable InconsistentNaming



namespace Final {
    internal static class Program {
        
        public static readonly string[] AlwaysSmall = { "AvgSpeed", "AvgPos", "Podiums", "First", "Second", "Third", "Runners"};
        public readonly struct Runner {
            
            private readonly int _id;
            public readonly int ID => _id;
           
            
            
            public readonly string Name;
            public readonly string Team;

            public readonly double SalaryPerRace;
            
            public readonly double TotalSalary;

            public readonly DateTime Birthday;

            
            public readonly int[] _PositionHistory;
            public readonly float[] _AvgVelocityHistory;

            public int Age {
                get {
                    var today = DateTime.Today;
                    var age = today.Year - Birthday.Year;
                    if (Birthday.Date > today.AddYears(-age)) age--;
                    return age;
                }
            }
            
            public readonly double AvgSpeed;
            public readonly double AvgPos;
            public readonly int Podiums;
            public readonly int First;
            public readonly int Second;
            public readonly int Third;

            private static double CalculateTotalSalary(double baseSalary, int[] positionHistory) {
                double result = baseSalary;

                foreach (var position in positionHistory) {
                    result += 50000/(double)position;
                }

                return Math.Round(result, 2);
            }


            public Runner(Runner[] alreadyRegistered, int numRegistered, int numberRaces, int numberRunners) {
                _id = numRegistered+1;
                while (true) {
                    Name = GetInput<string>($"\nIngresa el nombre del corredor {numRegistered + 1}: ");
                    if (FindElementIndex(alreadyRegistered, "Name", Name , new Tuple<int, int>(0, numRegistered)) == -1) break;
                    Console.WriteLine("ERROR: Un corredor con ese nombre ya está registrado");
                }
               
                Team = GetInput<string>("Ingresa el nombre del Team: ");
                SalaryPerRace =  GetInput<double>("Ingresa el salario base: $");
                while (true) {
                    try {
                        Birthday = DateTime.ParseExact(GetInput<string>("Ingresa la fecha de nacimiento (dd/MM/yyyy): "), "dd/MM/yyyy", null);
                        break;
                    } catch (Exception) {
                        Console.WriteLine("ERROR: Ingresa el formato correcto dd/MM/yyyy");
                    }
                }
                
                _PositionHistory = new int[numberRaces];
                _AvgVelocityHistory = new float[numberRaces];
                
                AvgPos = 0;
                Podiums = 0;
                First = 0;
                Second = 0;
                Third = 0;
                AvgSpeed = 0;
                
                

                for (var raceN=0; raceN < numberRaces; raceN++) {
                    
                    _PositionHistory[raceN] = GetInput($"Ingresa la posición del corredor en la carrera {raceN} (1-{numberRunners}): ",
                        new Tuple<int, int>(1, numberRunners));

                    bool wasRegistered = false;
                    
                    for (var pilotN=0; pilotN< numRegistered; pilotN++) {
                        if (alreadyRegistered[pilotN]._PositionHistory[raceN] != _PositionHistory[raceN]) continue;
                        Console.WriteLine($"ERROR: Otro corredor ya fue registrado con esa posición en la carrera {raceN}");
                        raceN--;
                        wasRegistered = true;
                        break;
                    }
                    
                    if (wasRegistered) continue;
                    
                    _AvgVelocityHistory[raceN] =
                        GetInput($"Ingresa la velocidad media del corredor en la carrera {raceN} (1-500): ",
                            new Tuple<float, float>(0, 500));
                    
                    AvgPos += _PositionHistory[raceN];
                    AvgSpeed += _AvgVelocityHistory[raceN];
                    
                    if (_PositionHistory[raceN] >= 1 && _PositionHistory[raceN] <= 3) Podiums++;

                    switch (_PositionHistory[raceN]) {
                        case 1: First++; break;
                        case 2: Second++; break;
                        case 3: Third++; break;
                    }
                }
                
                TotalSalary = CalculateTotalSalary(SalaryPerRace, _PositionHistory);
                AvgPos = Math.Round(AvgPos/numberRaces, 2);
                AvgSpeed = Math.Round(AvgSpeed/numberRaces, 2);
            }
        }
        
        public struct Team {
            
            private readonly int _id;
            public readonly int ID => _id;
            public readonly string Name;
            
            public readonly Runner[] _Runners;
            public int Runners;

            public double SalaryPerRace;
            
            public double TotalSalary;

            public double AvgSpeed;
            public double AvgPos;
            public int Podiums;
            public int First;
            public int Second;
            public int Third;

            private void RegisterMember(Runner runner) {
                
                _Runners[Runners] = runner;
                Runners += 1;
                
                TotalSalary = Math.Round(TotalSalary + runner.TotalSalary, 2);
                SalaryPerRace = Math.Round(SalaryPerRace + runner.SalaryPerRace, 2);
                Podiums += runner.Podiums;
                
                First += runner.First;
                Second += runner.Second;
                Third += runner.Third;
                
                AvgSpeed *= Runners-1;
                AvgSpeed += runner.AvgSpeed;
                AvgSpeed =  Math.Round(AvgSpeed/Runners, 2);
                
                AvgPos *= Runners-1;
                AvgPos += runner.AvgPos;
                AvgPos = Math.Round(AvgPos/Runners, 2);
            }

            public static void RegisterRunnerOrCreateTeamAndRegister(Team[] alreadyRegistered, Runner runner, int numberRunner, int numberOfRunners ) {
                int i;
                for (i=0; i< numberOfRunners; i++) {
                    if (alreadyRegistered[i]._id == 0 ) break;
                }
                int id = numberRunner > 0 ? alreadyRegistered[i-1]._id + 1 : 1;
                
                int pos = FindElementIndex(alreadyRegistered, "Name", runner.Team, inRange: new (0,id-1 ));
                
                if ( pos == -1 ) {
                    alreadyRegistered[id-1] = new Team(numberOfRunners, runner.Team, id );
                    alreadyRegistered[id-1].RegisterMember(runner);
                } else {
                    alreadyRegistered[pos].RegisterMember(runner);
                }

            }
            public Team(int numberRunners,  string name, int id) {
                _id = id;
                Name = name;
                
                Runners = 0;
                SalaryPerRace =  0;
                AvgPos = 0;
                Podiums = 0;
                First = 0;
                Second = 0;
                Third = 0;
                TotalSalary = 0;
                
                AvgSpeed = 0;
                AvgPos =0;
                
                _Runners = new Runner[numberRunners];
                
            }
        }

        private static void InputData(AppState state) {
            for (var i=0; i<state.NumberRunners; i++) {
                state.Runners[i] = new Runner(state.Runners, i,  state.NumberRaces, state.NumberRunners);
                Team.RegisterRunnerOrCreateTeamAndRegister(state.Teams, state.Runners[i], i, state.NumberRunners);
            }
        }
        
        private static void SubMenuResults<T>(T[] data)
        {
            while (true) {
                string campo;
            
                var option = AskForOption((1, 4),
                    "\nMenu de opciones:\n"+
                    "    1. Ordenar por campo\n"+
                    "    2. Buscar por campo\n" +
                    "    3. Mostrar la tabla\n"+
                    "    4. Salir\n"
                );
                
                if (option == 4) return;
                
                switch (option) {
                    case 1:
                        var operadorStr = AskForOption((1, 3),
                            "\nCómo deseas ordenarlo?\n" +
                            "    1. Mayor a menor\n"+
                            "    2. Menor a mayor\n"+
                            "    3. Cancelar"
                        );
                        
                        if (operadorStr == 3) break;
                        
                        campo = AskForFieldOrProperty<T>(
                            prompt:"Cuál es el campo por el que deseas ordenar los datos?: ",
                            except: new []{"Name", "Team"}
                        );

                        Operator operador = operadorStr == 1 ? Operator.Biggest : Operator.Smallest;
                    
                        Sort(data, campo, operador);
                        ShowData(data, small: AlwaysSmall);
                        break;
                    case 2: 
                        
                        campo = AskForFieldOrProperty<T>(
                            prompt:"Cuál es el campo por el que deseas filtrar los datos?: "
                        );
                        
                        bool strict = GetInput<string>("¿Deseas que la búsqueda sea estricta? [y/n]: ").ToLower() == "y";

                        FilterFieldProperty(data, campo, small: AlwaysSmall, strict: strict);

                        break;
                    case 3:
                        ShowData(data, small: AlwaysSmall);
                        break;
                }
            }
        }

        private struct AppState {
            public int NumberRaces;
            public int NumberRunners;
            public  Runner[] Runners;
            public  Team[] Teams;
        }

        private static void Main() {
            
            
            AppState state = new AppState { Runners = null, Teams =  null};
            
            while (true) {

                var option = AskForOption((1, 4),
                    "\nMenu de opciones:\n"+
                    "    1. Entrada de datos\n"+
                    "    2. Resultados Corredores\n" +
                    "    3. Resultados Equipos\n" + 
                    "    4. Salir"
                );
                
                if (option == 4) break;

                switch (option)
                {
                    case 1:
                        state.NumberRaces = GetInput("Ingresa el número de carreras de la temporada (6-25):",
                            new Tuple<int, int>(6, 25));
                        state.NumberRunners = GetInput("Ingresa el número de corredores en la competencia (6-26): ",
                            new Tuple<int, int>(6, 26));
                        state.Runners = new Runner[state.NumberRunners];
                        state.Teams = new Team[state.NumberRunners];
                            
                        InputData(state);
                        break;
                    case 2:
                        if (state.Runners != null) SubMenuResults(state.Runners);
                        else Console.WriteLine("ERROR: Debes ingresar registros para continuar (en la opción 1)");
                        break;
                    case 3:
                        if (state.Teams != null) SubMenuResults(state.Teams);
                        else Console.WriteLine("ERROR: Debes ingresar registros para continuar (en la opción 1)");
                        break;
                }
                
                
                
            }
            
        }
    }
}