using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Forms;

namespace MiniC
{
    public class Tokens
    {
        int _Linea;
        string _Lexema;
        int _Token;
        public Tokens(int linea, string lexema, int token)
        {
            _Linea = linea;
            _Lexema = lexema;
            _Token = token;
        }
        public int Linea { get => _Linea; set => _Linea = value; }
        public string Lexema { get => _Lexema; set => _Lexema = value; }
        public int Token { get => _Token; set => _Token = value; }
    }
    public class AnalizadorLexico
    {
        readonly UnidadesLexica UL = new UnidadesLexica();
        List<Tokens> LstTokens = new List<Tokens>(); // clonamos los objetos basados en las 2 clases previas
        readonly string L = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
        readonly string D = "0123456789";
        readonly string AR = "+-*/%";
        int Cont = 0; //variable global que cuenta en que posición del archivo me encuentro
        int Linea = 1; //variable global para el numero de linea
        string Lexema = string.Empty; //vacía el lexema cada que se completa un token

        public bool ModoConsola = false;
        void ReportarError(string mj)
        {
            if (ModoConsola) Console.WriteLine("LEXER ERROR: " + mj);
            else MessageBox.Show(mj);
        }

        protected int GetAlfabetoPalabra(char c)
        {
            if (L.Contains(c)) // si L le contiene a c significa que es letra
                return 0; // devuelve cero porque estoy en la columna cero de la TT
            else if (D.Contains(c))
                return 1; // si D contiene a c es un dígito retorna columna 1 de la TT
            else if (c == '_')
                return 2; // si c es un sugbuion entonces retorna la columna 2 de la TT
            return -1; // si c no coincide con ninguno retorna -1
        }
        protected int GetAlfabetoNumero(char c)
        {
            if (D.Contains(c)) // si L le contiene a c significa que es letra
                return 0;
            else if (c == '+')
                return 1;
            else if (c == '-')
                return 2;
            else if (c == '=')
                return 3;
            else if (c == '.')
                return 4;
            else if (c == 'E')
                return 5;
            else if (c == 'e')
                return 6;
            return -1; // si c no coincide con ninguno retorna -1
        }
        protected void AutomataPalabras(string Archivo)
        {
            char c;
            int Estado = 0; //estado de trancisión en el que nos encontramos
            int Simbolo; // representa el simbolo = mediante el indice de la columna de TT en la que nos encontramos
            string Lexema = string.Empty; // contiene el lexema pero se vacia antes de iniciar
            int[,] TT =
            {
                {1,-1,1}, // fila cero para estado cero
                {1,1,1} // fila uno para el estado 1 
            };
            do
            {
                c = Archivo[Cont]; // recibimo el simbolo en posicion Cont
                Simbolo = GetAlfabetoPalabra(c); // esta deberia reconocer que simbolo es y retornar el indice de la columna
                if (Simbolo == -1)
                    break; // si caemos en estado de rechazo termina ejecucion del automata
                Lexema += c; // si es un caracter reconocible por el automata lo añadimos al lexema
                Estado = TT[Estado, Simbolo]; // ejecutamos el automata (es decir actualizamos el estado acorde con lo que diga la tabla)
                Cont++;
            } while (Cont < Archivo.Length);
            if (Estado == 1) // si el ultimo estado en el que se quedo el automata es de aceptacion guarda en lista de tokens
            {
                LstTokens.Add(new Tokens(Linea, Lexema, UL.GetTokenPalabra(Lexema)));
                Lexema = string.Empty;
            }
        }

        protected void AutomataNumeros(string Archivo)
        {
            char c;
            int Estado = 0; //estado de trancisión en el que nos encontramos
            int Simbolo; // representa el simbolo = mediante el indice de la columna de TT en la que nos encontramos
            string Lexema = string.Empty; // contiene el lexema pero se vacia antes de iniciar
            int[,] TT =
            {
                {5,1,2,-1,4,-1,-1},//0
                {5,3,-1,3,4,-1,-1},//1
                {5,-1,3,3,4,-1,-1},//2
                {-1,-1,-1,-1,-1,-1,-1},//3
                {5,-1,-1,-1,-1,-1,-1},//4
                {5,-1,-1,-1,6,8,8},//5
                {7,-1,-1,-1,-1,-1,-1},//6
                {7,-1,-1,-1,-1,8,8},//7
                {10,9,9,-1,-1,-1,-1},//8
                {10,-1,-1,-1,-1,-1,-1 },//9
                {10,-1,-1,-1,-1,-1,-1 }
            };
            do
            {
                c = Archivo[Cont]; // recibimo el simbolo en posicion Cont
                Simbolo = GetAlfabetoNumero(c); // esta deberia reconocer que simbolo es y retornar el indice de la columna
                if (Simbolo == -1)
                    break; // si caemos en estado de rechazo termina ejecucion del automata
                Lexema += c; // si es un caracter reconocible por el automata lo añadimos al lexema
                Estado = TT[Estado, Simbolo]; // ejecutamos el automata (es decir actualizamos el estado acorde con lo que diga la tabla)
                Cont++;
            } while (Cont < Archivo.Length);
            if (Estado == 1 || Estado == 2 || Estado == 5 || Estado == 7 || Estado == 10) // si el ultimo estado en el que se quedo el automata es de aceptacion guarda en lista de tokens
            {
                if (Lexema.Contains(".")) //si tiene punto decimal es decimal entonces token 302
                {
                    LstTokens.Add(new Tokens(Linea, Lexema, 302));
                    Lexema = string.Empty;
                }
                else // sino es entero token 301
                {
                    LstTokens.Add(new Tokens(Linea, Lexema, 301));
                    Lexema = string.Empty;
                }
            }
            else if (Estado == 3)
            {
                LstTokens.Add(new Tokens(Linea, Lexema, UL.GetTokenSimbolo(Lexema)));
                Lexema = string.Empty;
            }
        }

        protected void AutomataString(string Archivo)
        {
            // Consumir comilla inicial
            Cont++;
            string str = "\"";
            while (Cont < Archivo.Length && Archivo[Cont] != '"')
            {
                if (Archivo[Cont] == '\\' && Cont + 1 < Archivo.Length)
                {
                    // Escape char
                    str += Archivo[Cont]; Cont++;
                    str += Archivo[Cont]; Cont++;
                }
                else
                {
                    str += Archivo[Cont];
                    Cont++;
                }
            }
            if (Cont < Archivo.Length && Archivo[Cont] == '"')
            {
                str += "\"";
                Cont++;
                LstTokens.Add(new Tokens(Linea, str, 303)); // 303 = String Constante
            }
            Lexema = string.Empty;
        }

        protected void AutomataCaracter(string Archivo)
        {
            // Consumir comilla inicial
            Cont++;
            string str = "'";
            while (Cont < Archivo.Length && Archivo[Cont] != '\'')
            {
                if (Archivo[Cont] == '\\' && Cont + 1 < Archivo.Length)
                {
                    // Escape char
                    str += Archivo[Cont]; Cont++;
                    str += Archivo[Cont]; Cont++;
                }
                else
                {
                    str += Archivo[Cont];
                    Cont++;
                }
            }
            if (Cont < Archivo.Length && Archivo[Cont] == '\'')
            {
                str += "'";
                Cont++;
                LstTokens.Add(new Tokens(Linea, str, 306)); // 306 = Constante Caracter
            }
            Lexema = string.Empty;
        }

        protected void AutomataOperadores(string Archivo)
        {
            string Lexema = string.Empty;
            char c = Archivo[Cont];
            Lexema += c;
            Cont++;

            // Lookahead 1 char for 2-char operators (or 3 for ...)
            if (Cont < Archivo.Length)
            {
                char c2 = Archivo[Cont];
                string BiLexema = Lexema + c2;

                // Special case: ...
                if (BiLexema == ".." && Cont + 1 < Archivo.Length && Archivo[Cont + 1] == '.')
                {
                    Lexema += c2; Cont++;
                    Lexema += Archivo[Cont]; Cont++;
                    LstTokens.Add(new Tokens(Linea, Lexema, UL.GetTokenSimbolo(Lexema)));
                    return;
                }

                // COMENTARIOS
                if (BiLexema == "//")
                {
                    Cont++; // Consumir segundo /
                    while (Cont < Archivo.Length && Archivo[Cont] != '\n')
                    {
                        Cont++;
                    }
                    return; // No generar token
                }
                if (BiLexema == "/*")
                {
                    Cont++; // Consumir *
                    while (Cont + 1 < Archivo.Length)
                    {
                        if (Archivo[Cont] == '*' && Archivo[Cont + 1] == '/')
                        {
                            Cont += 2; // Consumir */
                            return;
                        }
                        if (Archivo[Cont] == '\n') Linea++;
                        Cont++;
                    }
                    return;
                }

                if (UL.GetTokenSimbolo(BiLexema) != -1)
                {
                    // It matches a 2-char token
                    Lexema = BiLexema;
                    Cont++;
                    LstTokens.Add(new Tokens(Linea, Lexema, UL.GetTokenSimbolo(Lexema)));
                    return;
                }
            }

            // If not 2-char match (or EOF), check 1-char
            // Note: + and - might be signs, but here we token them as ops.
            int tk = UL.GetTokenSimbolo(Lexema);
            if (tk != -1)
            {
                LstTokens.Add(new Tokens(Linea, Lexema, tk));
            }
            else
            {
                ReportarError("Simbolo no reconocido: " + Lexema);
            }
        }

        public List<Tokens> AnalisisLexico(string Archivo)
        {
            char c;
            while (Cont < Archivo.Length)
            {
                c = Archivo[Cont];
                // Automatas de 2 estados, tan sencillos que no necesito Tabla Transición
                if ("\n\r".Contains(c))
                {
                    Linea++; Cont++;
                }
                else if ("\t\0 ".Contains(c))
                {
                    Cont++;
                }
                //Autómatas de un solo símbolo (strictly single char tokens)
                else if ("()[]{},;:#".Contains(c))
                {
                    Lexema += c;
                    LstTokens.Add(new Tokens(Linea, Lexema, UL.GetTokenSimbolo(Lexema)));
                    Lexema = string.Empty;
                    Cont++;
                }
                else if (c == '_' || L.Contains(c))
                {
                    AutomataPalabras(Archivo);
                }
                else if (D.Contains(c))
                {
                    AutomataNumeros(Archivo);
                }
                else if (c == '.' && Cont + 1 < Archivo.Length && D.Contains(Archivo[Cont + 1]))
                {
                    // Float starting with dot (.5)
                    AutomataNumeros(Archivo);
                }
                else if ("=<>!&|^+-*/%?.".Contains(c))
                {
                    AutomataOperadores(Archivo);
                }
                else if (c == '"')
                {
                    AutomataString(Archivo);
                }
                else if (c == '\'')
                {
                    AutomataCaracter(Archivo);
                }
                else
                {
                    // Character unknown/ignored to prevent infinite loop
                    Cont++;
                }
            }
            return LstTokens;
        }
    }

    public class AnalizadorSintactico
    {
        List<Tokens> ListaTokens = new List<Tokens>();
        int Cont = 0;
        int Token;
        public bool Compilacion = true;

        // Helper para reportar errores
        public bool ModoConsola = false;
        protected void ReportarError(string mensaje)
        {
            Compilacion = false;
            if (ModoConsola) Console.WriteLine(mensaje);
            else MessageBox.Show(mensaje);
        }

        protected bool Parea(int Tk, string Lexema) //recibe el num token y el lexema y verifica si es el token esperado
        {
            if (Cont >= ListaTokens.Count) return false;
            if (ListaTokens[Cont].Token == Tk)
            {
                Cont++;
                return true;
            }
            else
            {
                ReportarError("Error sintáctico: Se esperaba " + Lexema + " en la línea " + ((Cont < ListaTokens.Count) ? ListaTokens[Cont].Linea.ToString() : "?"));
                return false;
            }
        }


        protected void gramatica_declaracion_variable()
        {
            if (Cont >= ListaTokens.Count) return;

            // Soporte para punteros (uno o más sumbolos *)
            while (Cont < ListaTokens.Count && ListaTokens[Cont].Token == 42) // 42 es *
            {
                Cont++;
            }

            if (Parea(300, "Identificador"))
            {
                gramatica_declarador_suffix();

                // Soporte para variables múltiples: int a, b = 2, c;
                while (Cont < ListaTokens.Count && ListaTokens[Cont].Token == 91) // ,
                {
                    Cont++; // Consumir coma

                    // Punteros opcionales para la siguiente variable
                    while (Cont < ListaTokens.Count && ListaTokens[Cont].Token == 42)
                    {
                        Cont++;
                    }

                    Parea(300, "Identificador");
                    gramatica_declarador_suffix();
                }

                if (Parea(93, ";"))
                    return;
            }
        }

        protected void gramatica_declarador_suffix()
        {
            // Soporte para arreglos estáticos unidimensionales: [ numero ]
            if (Cont < ListaTokens.Count && ListaTokens[Cont].Token == 77) // [
            {
                Parea(77, "[");
                // Esperamos un entero constante (o expresión constante)
                if (Cont < ListaTokens.Count && ListaTokens[Cont].Token == 301) // entero decimal
                {
                    Cont++;
                }
                Parea(78, "]");
            }

            // Soporte para inicialización: = valor  o  = { ... }
            if (Cont < ListaTokens.Count && ListaTokens[Cont].Token == 50) // =
            {
                Cont++;

                if (Cont < ListaTokens.Count && ListaTokens[Cont].Token == 79) // {
                {
                    // Inicializador de agregado (struct/array)
                    Parea(79, "{");
                    if (ListaTokens[Cont].Token != 80) // Si no es } vacío
                    {
                        gramatica_inicializador();
                        while (Cont < ListaTokens.Count && ListaTokens[Cont].Token == 91) // ,
                        {
                            Cont++;
                            gramatica_inicializador();
                        }
                    }
                    Parea(80, "}");
                }
                else
                {
                    // Expresion simple
                    gramatica_expresion_constante();
                }
            }
        }

        protected void gramatica_declaracion_struct()
        {
            if (Cont >= ListaTokens.Count) return;

            // Opcional: Nombre de la estructura
            if (Cont < ListaTokens.Count && ListaTokens[Cont].Token == 300)
            {
                Cont++;
            }

            // Cuerpo de la estructura
            if (Cont < ListaTokens.Count && ListaTokens[Cont].Token == 79) // {
            {
                Parea(79, "{");
                // Declaraciones de miembros
                while (Cont < ListaTokens.Count && ListaTokens[Cont].Token != 80) // }
                {
                    int tk = ListaTokens[Cont].Token;
                    if (nombre_tipo(tk)) // int, float, char...
                    {
                        Cont++;
                        gramatica_declaracion_variable();
                    }
                    else if (tk == 25) // struct anidado
                    {
                        Cont++;
                        gramatica_declaracion_struct();
                    }
                    else
                    {
                        // Evitar bucle infinito si hay error
                        Cont++;
                    }
                }
                Parea(80, "}");
            }

            // Variables de tipo structure al final de la definición: struct Point { .. } p1;
            if (Cont < ListaTokens.Count && ListaTokens[Cont].Token == 300)
            {
                gramatica_declaracion_variable();
            }
            else
            {
                Parea(93, ";");
            }
        }

        protected void gramatica_declaracion_enum()
        {
            if (Cont >= ListaTokens.Count) return;

            // Opcional: Nombre del enum
            if (Cont < ListaTokens.Count && ListaTokens[Cont].Token == 300)
            {
                Cont++;
            }

            if (Parea(79, "{"))
            {
                // Lista de enumeradores
                while (Cont < ListaTokens.Count)
                {
                    Parea(300, "Identificador"); // Nombre del valor enum

                    // Opcional: Asignación de valor = constante
                    if (Cont < ListaTokens.Count && ListaTokens[Cont].Token == 50) // =
                    {
                        Cont++;
                        if (Cont < ListaTokens.Count && ListaTokens[Cont].Token == 301) // entero
                            Cont++;
                    }

                    if (Cont < ListaTokens.Count && ListaTokens[Cont].Token == 91) // ,
                    {
                        Cont++;
                        continue;
                    }
                    else
                    {
                        break; // Si no hay coma, terminamos
                    }
                }
                Parea(80, "}");
            }
            Parea(93, ";");
        }

        protected void gramatica_declaracion_typedef()
        {
            if (Cont >= ListaTokens.Count) return;

            int tk = ListaTokens[Cont].Token;

            // typedef [Type] [Alias];
            // El Type puede ser simple (int) o struct/enum
            if (nombre_tipo(tk))
            {
                Cont++;
                gramatica_declaracion_variable(); // Reutilizamos lógica: espera ID y ;
            }
            else if (tk == 25) // struct
            {
                Cont++;
                gramatica_declaracion_struct();
            }
            else if (tk == 11) // enum
            {
                Cont++;
                gramatica_declaracion_enum();
            }
            else
            {
                MessageBox.Show("Error sintáctico en typedef: Se esperaba un tipo.");
            }
        }

        private bool EsInicioDeclaracion(int tk)
        {
            return nombre_tipo(tk) || tk == 25 || tk == 28 || tk == 11 || tk == 27;
        }

        protected void gramatica_bloque_sentencias()
        {
            try
            {
                if (!Parea(79, "{")) return;

                // Declaraciones y Sentencias mezcladas (C99)
                while (Cont < ListaTokens.Count && ListaTokens[Cont].Token != 80) // 80 == '}'
                {
                    int prevCont = Cont;
                    int tk = ListaTokens[Cont].Token;

                    if (EsInicioDeclaracion(tk))
                    {
                        gramatica_declaracion_local();
                    }
                    else
                    {
                        gramatica_sentencia();
                    }

                    if (prevCont == Cont && Cont < ListaTokens.Count && ListaTokens[Cont].Token != 80)
                    {
                        Cont++; // Evitar bucle infinito
                    }
                }

                Parea(80, "}");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Excepción en bloque: " + ex.Message);
            }
        }

        protected void gramatica_declaracion_local()
        {
            if (Cont >= ListaTokens.Count) return;
            int tk = ListaTokens[Cont].Token;

            if (tk == 25) // struct
            {
                Cont++; gramatica_declaracion_struct();
            }
            else if (tk == 28) // union
            {
                Cont++; gramatica_declaracion_union();
            }
            else if (tk == 11) // enum
            {
                Cont++; gramatica_declaracion_enum();
            }
            else if (tk == 27) // typedef
            {
                Cont++; gramatica_declaracion_typedef();
            }
            else if (nombre_tipo(tk))
            {
                Cont++; gramatica_declaracion_variable();
            }
            else
            {
                // Error o avanzar
                Cont++;
            }
        }

        protected void gramatica_declaracion_union()
        {
            // Similar a struct pero con palabra reservada union
            if (Cont >= ListaTokens.Count) return;

            if (Cont < ListaTokens.Count && ListaTokens[Cont].Token == 300) Cont++; // Nombre union

            if (Cont < ListaTokens.Count && ListaTokens[Cont].Token == 79) // {
            {
                Parea(79, "{");
                while (Cont < ListaTokens.Count && ListaTokens[Cont].Token != 80)
                {
                    // Miembros
                    gramatica_declaracion_variable_miembro();
                }
                Parea(80, "}");
            }

            if (Cont < ListaTokens.Count && ListaTokens[Cont].Token == 300)
            {
                gramatica_declaracion_variable(); // Variable de tipo union
            }
            else
            {
                Parea(93, ";");
            }
        }

        protected void gramatica_declaracion_variable_miembro()
        {
            // Simplificado para miembros de struct/union
            if (nombre_tipo(ListaTokens[Cont].Token))
            {
                Cont++;
                gramatica_declaracion_variable();
            }
            else if (ListaTokens[Cont].Token == 25) // nested struct
            {
                Cont++; gramatica_declaracion_struct();
            }
            else if (ListaTokens[Cont].Token == 28) // nested union
            {
                Cont++; gramatica_declaracion_union();
            }
            else
            {
                Cont++; // Evitar bucle
            }
        }

        protected void gramatica_sentencia()
        {
            if (Cont >= ListaTokens.Count) return;
            int tk = ListaTokens[Cont].Token;

            switch (tk)
            {
                case 16: // if
                    gramatica_sentencia_if();
                    break;
                case 32: // while
                    gramatica_sentencia_while();
                    break;
                case 8: // do
                    gramatica_sentencia_do();
                    break;
                case 14: // for
                    gramatica_sentencia_for();
                    break;
                case 26: // switch
                    gramatica_sentencia_switch();
                    break;
                case 20: // return
                    gramatica_sentencia_return();
                    break;
                case 2: // break
                    gramatica_sentencia_break();
                    break;
                case 6: // continue
                    gramatica_sentencia_continue();
                    break;
                case 79: // { Bloque anidado
                    gramatica_bloque_sentencias();
                    break;
                case 93: // ; Sentencia vacía
                    Cont++;
                    break;
                default:
                    // Sentencia expresión
                    gramatica_expresion();
                    Parea(93, ";");
                    break;
            }
        }

        protected void gramatica_sentencia_if()
        {
            // if ( expresion ) sentencia [else sentencia]
            Parea(16, "if");
            Parea(75, "(");
            gramatica_expresion();
            Parea(76, ")");
            gramatica_sentencia();

            if (Cont < ListaTokens.Count && ListaTokens[Cont].Token == 10) // else
            {
                Cont++;
                gramatica_sentencia();
            }
        }

        protected void gramatica_sentencia_while()
        {
            // while ( expresion ) sentencia
            Parea(32, "while");
            Parea(75, "(");
            gramatica_expresion();
            Parea(76, ")");
            gramatica_sentencia();
        }

        protected void gramatica_sentencia_do()
        {
            // do sentencia while ( expresion ) ;
            Parea(8, "do");
            gramatica_sentencia();
            Parea(32, "while");
            Parea(75, "(");
            gramatica_expresion();
            Parea(76, ")");
            Parea(93, ";");
        }

        protected void gramatica_sentencia_for()
        {
            // for ( exp_opt ; exp_opt ; exp_opt ) sentencia
            Parea(14, "for");
            Parea(75, "(");

            // Support C99: for (int i=0; ...)
            if (nombre_tipo(ListaTokens[Cont].Token))
            {
                Cont++; // Consumir el tipo (int, float, etc)
                gramatica_declaracion_variable();
                // gramatica_declaracion_variable consumes the semicolon usually
            }
            else
            {
                if (ListaTokens[Cont].Token != 93) gramatica_expresion();
                Parea(93, ";");
            }

            if (ListaTokens[Cont].Token != 93) gramatica_expresion();
            Parea(93, ";");

            if (ListaTokens[Cont].Token != 76) gramatica_expresion();
            Parea(76, ")");

            gramatica_sentencia();
        }

        protected void gramatica_sentencia_switch()
        {
            Parea(26, "switch");
            Parea(75, "(");
            gramatica_expresion();
            Parea(76, ")");
            Parea(79, "{");

            // Cuerpo del switch: case / default
            while (Cont < ListaTokens.Count && ListaTokens[Cont].Token != 80)
            {
                if (ListaTokens[Cont].Token == 3) // case
                {
                    Cont++;
                    gramatica_expresion_constante();
                    Parea(92, ":");
                    // Sentencias dentro del case
                    while (Cont < ListaTokens.Count &&
                           ListaTokens[Cont].Token != 3 && // case
                           ListaTokens[Cont].Token != 7 && // default
                           ListaTokens[Cont].Token != 80)  // }
                    {
                        gramatica_sentencia();
                    }
                }
                else if (ListaTokens[Cont].Token == 7) // default
                {
                    Cont++;
                    Parea(92, ":");
                    while (Cont < ListaTokens.Count &&
                           ListaTokens[Cont].Token != 3 &&
                           ListaTokens[Cont].Token != 7 &&
                           ListaTokens[Cont].Token != 80)
                    {
                        gramatica_sentencia();
                    }
                }
                else
                {
                    // Si hay algo que no es case/default dentro de switch, suele ser error,
                    // pero a veces se permite código muerto o labels. Asumimos error o saltamos.
                    Cont++;
                }
            }
            Parea(80, "}");
        }

        protected void gramatica_sentencia_return()
        {
            Parea(20, "return");
            if (ListaTokens[Cont].Token != 93) // Si no es ;
            {
                gramatica_expresion();
            }
            Parea(93, ";");
        }

        protected void gramatica_sentencia_break()
        {
            Parea(2, "break");
            Parea(93, ";");
        }

        protected void gramatica_sentencia_continue()
        {
            Parea(6, "continue");
            Parea(93, ";");
        }
        protected void gramatica_expresion_primaria()
        {
            if (Cont >= ListaTokens.Count) { Compilacion = false; return; }
            int tk = ListaTokens[Cont].Token;

            // IDENTIFICADOR: Token 300
            if (tk == 300)
            {
                Parea(300, "Identificador");
            }
            // Funciones main (220) o standard (120-213) usadas como Factor
            else if (tk == 220 || (tk >= 120 && tk <= 213))
            {
                // Consumimos el token de la función (ej. printf)
                Cont++;
            }
            // Constantes numéricas
            else if (tk == 301 || tk == 302 || tk == 304 || tk == 305 || tk == 306)
            {
                gramatica_constante();
            }
            // STRING_CONSTANTE
            else if (tk == 303)
            {
                gramatica_string_constante();
            }
            // ( EXPRESION )
            else if (Parea(75, "(")) // Token 75 es '('
            {
                gramatica_expresion(); // Allow full expression inside parens
                Parea(76, ")"); // Token 76 es ')'
            }
            else
            {
                ReportarError("Error sintáctico: Se esperaba una expresión primaria en la línea " + (Cont < ListaTokens.Count ? ListaTokens[Cont].Linea.ToString() : "?"));
                return;
            }
        }

        protected void gramatica_expresion_posfija()
        {
            gramatica_expresion_primaria();

            bool continuar = true;
            while (continuar && Cont < ListaTokens.Count)
            {
                int tk = ListaTokens[Cont].Token;

                switch (tk)
                {
                    case 77: // [
                        Parea(77, "[");
                        gramatica_expresion();
                        Parea(78, "]");
                        break;

                    case 75: // (
                        Parea(75, "(");
                        if (Cont < ListaTokens.Count && ListaTokens[Cont].Token != 76)
                        { // Si hay argumentos
                            gramatica_expresion_asignacion();
                            while (Cont < ListaTokens.Count && ListaTokens[Cont].Token == 91)
                            { // ,
                                Parea(91, ",");
                                gramatica_expresion_asignacion();
                            }
                        }
                        Parea(76, ")");
                        break;

                    case 90: // .
                        Parea(90, ".");
                        Parea(300, "Identificador");
                        break;

                    case 96: // ->
                        Parea(96, "->");
                        Parea(300, "Identificador");
                        break;

                    case 51: //++
                        Parea(51, "++");
                        break;
                    case 52: //-- 
                        Parea(52, "--");
                        break;

                    default:
                        continuar = false;
                        break;
                }
            }
        }
        protected void gramatica_constante()
        {
            if (Cont >= ListaTokens.Count) return;
            // Obtenemos el token actual para decidir a qué sub-gramática ir
            Token = ListaTokens[Cont].Token;

            switch (Token)
            {
                case 301: // El Lexer ya validó: Digitos + Sufijos (u, l, UL, etc.)
                    gramatica_constante_int_decimal();
                    break;

                case 302: // El Lexer ya validó: Digitos + Punto + Exponente + Sufijos (f, L)
                    gramatica_constante_float();
                    break;

                case 303: // El Lexer ya validó: " + Caracteres + "
                    gramatica_string_constante();
                    break;

                case 304: // El Lexer ya validó: 0 + Digitos Octales + Sufijos
                    gramatica_constante_int_octal();
                    break;

                case 305: // El Lexer ya validó: 0x + Digitos Hexa + Sufijos
                    gramatica_constante_int_hexadecimal();
                    break;

                case 306: // El Lexer ya validó: ' + caracter + '
                    gramatica_constante_caracter();
                    break;

                default:
                    // Si no es ninguna de las anteriores, no es una constante válida
                    break;
            }
        }
        protected void gramatica_string_constante()
        {
            Parea(303, "String Constante");
        }

        protected void gramatica_constante_caracter()
        {
            Parea(306, "Constante Carácter");
        }

        protected void gramatica_constante_int_decimal()
        {
            Parea(301, "Constante Entera Decimal");
        }

        protected void gramatica_constante_int_octal()
        {
            Parea(304, "Constante Entera Octal");
        }

        protected void gramatica_constante_int_hexadecimal()
        {
            Parea(305, "Constante Entera Hexadecimal");
        }

        protected void gramatica_constante_float()
        {
            Parea(302, "Constante de Punto Flotante");
        }
        protected Boolean nombre_tipo(int proximoTk)
        {
            // Devuelve true si el token corresponde a un "nombre de tipo"
            // Incluye: char(4), short(21), int(17), long(18), signed(22), unsigned(29),
            // float(13), double(9), const(5), volatile(31), void(30),
            // struct(25), enum(11), typedef(27)
            int[] tipos = { 4, 21, 17, 18, 22, 29, 13, 9, 5, 31, 30, 25, 11, 27 };
            return tipos.Contains(proximoTk);
        }
        protected void gramatica_expresion_unaria()
        {
            if (Cont >= ListaTokens.Count) { Compilacion = false; return; }
            int tk = ListaTokens[Cont].Token;
            // ++ o -- prefijo
            if (tk == 51 || tk == 52)
            {
                Cont++;
                gramatica_expresion_unaria();
                return;
            }
            // Operadores unarios: & (94), * (42), + (40), - (41), ! (72)
            else if (tk == 94 || tk == 42 || tk == 40 || tk == 41 || tk == 72)
            {
                Cont++;
                gramatica_expresion_cast();
                return;
            }
            // sizeof (23)
            else if (tk == 23)
            {
                Parea(23, "sizeof");
                if (Cont < ListaTokens.Count && ListaTokens[Cont].Token == 75)
                { // sizeof(tipo) o sizeof(exp)
                    if (Cont + 1 < ListaTokens.Count)
                    {
                        int proximoTk = ListaTokens[Cont + 1].Token;
                        if (nombre_tipo(proximoTk))
                        {
                            Parea(75, "(");
                            Cont++; // consume el tipo token (ya validado con nombre_tipo)
                            Parea(76, ")");
                            return;
                        }
                    }
                }
                else
                {
                    gramatica_expresion_unaria();
                    return;
                }
            }

            gramatica_expresion_posfija();
        }

        protected void gramatica_expresion_cast()
        {
            if (Cont >= ListaTokens.Count) { Compilacion = false; return; }
            // Verifica si es un inicio de Cast: ( tipo )
            if (ListaTokens[Cont].Token == 75)
            { // (
                if (Cont + 1 < ListaTokens.Count)
                {
                    int proximoTk = ListaTokens[Cont + 1].Token;
                    // Si el siguiente token es un tipo
                    if (nombre_tipo(proximoTk))
                    {
                        Parea(75, "(");
                        Cont++; // consume el token de tipo
                        Parea(76, ")");
                        gramatica_expresion_cast();
                        return;
                    }
                }
            }
            gramatica_expresion_unaria();
        }

        protected void gramatica_asignacion()
        {
            if (Cont >= ListaTokens.Count) return;
            // IDENTIFICADOR [= expresion]
            if (Parea(300, "Identificador"))
            {
                if (Cont < ListaTokens.Count)
                {
                    int tk = ListaTokens[Cont].Token;
                    if (EsOperadorAsignacion(tk))
                    {
                        Cont++; // consume operador de asignación
                        gramatica_expresion_asignacion();
                    }
                }
            }
        }

        // Multiplicación, División y Módulo (*, /, %)
        protected void gramatica_expresion_multiplicativa()
        {
            gramatica_expresion_cast();

            // Mientras el token actual sea un operador multiplicativo
            while (Compilacion && Cont < ListaTokens.Count && (ListaTokens[Cont].Token == 42 || ListaTokens[Cont].Token == 43 || ListaTokens[Cont].Token == 44))
            {
                Cont++; // Consumimos *, / o %
                gramatica_expresion_cast();
            }
        }
        protected void gramatica_expresion_relacional()
        {
            gramatica_expresion_desplazamiento();

            while (Compilacion && Cont < ListaTokens.Count && (ListaTokens[Cont].Token == 60 || ListaTokens[Cont].Token == 61 || ListaTokens[Cont].Token == 62 || ListaTokens[Cont].Token == 63))
            {
                Cont++; // Consumimos > <= >= <
                gramatica_expresion_desplazamiento();
            }
        }
        protected void gramatica_expresion_igualdad()
        {
            gramatica_expresion_relacional();

            while (Compilacion && Cont < ListaTokens.Count && (ListaTokens[Cont].Token == 64 || ListaTokens[Cont].Token == 65))
            {
                Cont++; // Consumimos == !=
                gramatica_expresion_relacional();
            }
        }
        protected void gramatica_expresion_and()
        {
            gramatica_expresion_igualdad();

            while (Compilacion && Cont < ListaTokens.Count && (ListaTokens[Cont].Token == 94))
            {
                Cont++; // Consumimos &
                gramatica_expresion_igualdad();
            }
        }
        protected void gramatica_expresion_o_exclusivo()
        {
            gramatica_expresion_and();

            while (Compilacion && Cont < ListaTokens.Count && (ListaTokens[Cont].Token == 45))
            {
                Cont++; // Consumimos ^
                gramatica_expresion_and();
            }
        }
        protected void gramatica_expresion_o_inclusivo()
        {
            gramatica_expresion_o_exclusivo();

            while (Compilacion && Cont < ListaTokens.Count && (ListaTokens[Cont].Token == 223))
            {
                Cont++; // Consumimos |
                gramatica_expresion_o_exclusivo();
            }
        }
        protected void gramatica_expresion_and_logica()
        {
            gramatica_expresion_o_inclusivo();

            while (Compilacion && Cont < ListaTokens.Count && (ListaTokens[Cont].Token == 70))
            {
                Cont++; // Consumimos &&
                gramatica_expresion_o_inclusivo();
            }
        }
        protected void gramatica_expresion_o_logica()
        {
            gramatica_expresion_and_logica();

            while (Compilacion && Cont < ListaTokens.Count && (ListaTokens[Cont].Token == 71))
            {
                Cont++; // Consumimos ||
                gramatica_expresion_and_logica();
            }
        }
        protected void gramatica_expresion_condicional()
        {
            gramatica_expresion_o_logica();
            if (Cont < ListaTokens.Count && ListaTokens[Cont].Token == 222)
            {
                Cont++; // consume '?'
                gramatica_expresion_o_logica();
                if (Parea(223, ":"))
                {
                    gramatica_expresion_condicional();
                }
            }

        }
        protected void gramatica_expresion_asignacion()
        {
            // expression := conditional-expression | unary-expression assignment-op expression
            gramatica_expresion_condicional();

            if (Cont < ListaTokens.Count)
            {
                int tk = ListaTokens[Cont].Token;
                if (EsOperadorAsignacion(tk))
                {
                    Cont++; // consume operador de asignacion
                    gramatica_expresion_asignacion();
                }
            }
        }
        protected void gramatica_expresion()
        {
            // Evitar recursión infinita: delegar a la regla de asignación
            gramatica_expresion_asignacion();
        }
        protected void gramatica_expresion_constante()
        {
            gramatica_expresion_condicional();
        }

        // Suma y Resta (+, -)
        protected void gramatica_expresion_aditiva()
        {
            gramatica_expresion_multiplicativa();

            // Mientras el token sea + o -
            while (Compilacion && Cont < ListaTokens.Count && (ListaTokens[Cont].Token == 40 || ListaTokens[Cont].Token == 41))
            {
                Cont++; // Consumimos + o -
                gramatica_expresion_multiplicativa();
            }
        }

        protected void gramatica_expresion_desplazamiento()
        {
            gramatica_expresion_aditiva();
            while (Compilacion && Cont < ListaTokens.Count && (ListaTokens[Cont].Token == 97 || ListaTokens[Cont].Token == 98))
            {
                Cont++; // Consume << o >>                                                                  
                gramatica_expresion_aditiva();
            }
        }
        protected void gramatica_parametros()
        {
            if (Cont >= ListaTokens.Count) return;
            if (ListaTokens[Cont].Token == 76) return; // ')' lista vacía

            do
            {
                // ... (elipsis)
                if (ListaTokens[Cont].Token == 221)
                {
                    Cont++;
                    break; // ... debe ser lo último
                }

                // Tipo
                if (nombre_tipo(ListaTokens[Cont].Token))
                {
                    Cont++;
                    // Punteros
                    while (Cont < ListaTokens.Count && ListaTokens[Cont].Token == 42) Cont++;

                    // ID (opcional en prototipos, obligatorio en definición normalmente, pero permitamos abstracto)
                    if (Cont < ListaTokens.Count && ListaTokens[Cont].Token == 300)
                    {
                        Cont++;
                        // Arrays int a[]
                        if (Cont < ListaTokens.Count && ListaTokens[Cont].Token == 77)
                        {
                            Cont++; // [
                            Parea(78, "]");
                        }
                    }
                }

                if (Cont < ListaTokens.Count && ListaTokens[Cont].Token == 91) // ,
                {
                    Cont++;
                }
                else
                {
                    break;
                }

            } while (Cont < ListaTokens.Count);
        }

        protected void gramatica_inicializador()
        {
            if (Cont >= ListaTokens.Count) return;

            if (ListaTokens[Cont].Token == 79) // { Nested
            {
                Cont++;
                if (ListaTokens[Cont].Token != 80)
                {
                    gramatica_inicializador();
                    while (Cont < ListaTokens.Count && ListaTokens[Cont].Token == 91)
                    {
                        Cont++;
                        gramatica_inicializador();
                    }
                }
                Parea(80, "}");
            }
            else
            {
                gramatica_expresion_constante();
            }
        }

        protected void gramatica_declaracion_global()
        {
            if (Cont >= ListaTokens.Count) return;

            // Modificadores (extern, static)
            while (Cont < ListaTokens.Count && (ListaTokens[Cont].Token == 12 || ListaTokens[Cont].Token == 24))
            {
                Cont++;
            }

            // Tipo
            if (Cont < ListaTokens.Count && nombre_tipo(ListaTokens[Cont].Token)) // int, void, struct...
            {
                Cont++;
            }
            // O caso struct declarada inline: struct { ... }
            else if (Cont < ListaTokens.Count && ListaTokens[Cont].Token == 25)
            {
                // struct anónimo o struct ID... manejarlo es complejo aquí, 
                // asumo simple nombre_tipo o "struct ID" que nombre_tipo no cubre si no es token 25 directo?
                // nombre_tipo incluye 25 (struct)
                // Pero mi nombre_tipo consume el token? No, solo chequea.
                // Arriba ya hice Cont++.
                // Si era struct y no se consumió en el if anterior...
                // Un momento, nombre_tipo(25) es true. Así que entra al if y hace Cont++.

                // Si es struct, podría haber cuerpo {...}.
                // Para simplificar: asumimos que "struct ID" se comporta como tipo, 
                // y "struct { ... }" también.
                // Si hubo definición struct {...}, ya se comió los tokens? No, gramatica_declaracion_global 
                // espera "Type ID" o "Type * ID".

                // Si el usuario escribe "struct Point { int x; };", eso es una declaracion de tipo, no global var/func (a menos que siga ID).
                // AnalisisSintactico tiene branch para struct. Entra aquí si es "struct Point p;"
            }

            // Punteros
            while (Cont < ListaTokens.Count && ListaTokens[Cont].Token == 42) Cont++;

            // Identificador o Main
            int tkIdentificador = (Cont < ListaTokens.Count) ? ListaTokens[Cont].Token : -1;
            if (tkIdentificador == 300 || tkIdentificador == 220) // ID o main
            {
                Cont++; // Parea manual para ambos casos

                // Si sigue '(', es función
                if (Cont < ListaTokens.Count && ListaTokens[Cont].Token == 75) // (
                {
                    Cont++; // consumir (
                    gramatica_parametros();
                    Parea(76, ")");

                    // Si sigue {, es definición. Si sigue ;, es prototipo.
                    if (Cont < ListaTokens.Count && ListaTokens[Cont].Token == 79) // {
                    {
                        gramatica_bloque_sentencias();
                    }
                    else
                    {
                        Parea(93, ";");
                    }
                }
                else
                {
                    // Es variable global
                    // Arrays
                    if (Cont < ListaTokens.Count && ListaTokens[Cont].Token == 77)
                    {
                        Cont++; Parea(78, "]");
                        // TODO: Tamaño array
                    }
                    // Init
                    if (Cont < ListaTokens.Count && ListaTokens[Cont].Token == 50)
                    {
                        Cont++; gramatica_expresion_constante();
                    }
                    Parea(93, ";");
                }
            }
        }

        public void AnalisisSintactico(List<Tokens> Lista)
        {
            ListaTokens = Lista;
            while (Cont < ListaTokens.Count && Compilacion)
            {
                Token = ListaTokens[Cont].Token;

                // Ignorar punto y coma extra a nivel global
                if (Token == 93)
                {
                    Cont++;
                    continue;
                }

                // Tipos, extern, static, void, struct, enum
                bool esTipo = nombre_tipo(Token);
                bool esModificador = (Token == 12 || Token == 24); // extern, static

                if (esTipo || esModificador)
                {
                    gramatica_declaracion_global();
                }
                else if (Token == 25) // struct (definición de tipo)
                {
                    Cont++; gramatica_declaracion_struct();
                }
                else if (Token == 11) // enum (definición de tipo)
                {
                    Cont++; gramatica_declaracion_enum();
                }
                else if (Token == 27) // typedef
                {
                    Cont++; gramatica_declaracion_typedef();
                }
                else
                {
                    // Si no reconocemos nada, avanzamos para evitar bucle (o lanzamos error)
                    // ReportarError("Token inesperado en declaracion global: " + Token);
                    Cont++;
                }
            }
        }

        // Helper para reconocer operadores de asignación
        bool EsOperadorAsignacion(int tk)
        {
            // =, >>=, <<=, +=, -=, *=, %=, /=, ^=, |=, &=
            return tk == 50 || tk == 98 || tk == 97 || tk == 53 || tk == 54 || tk == 55 || tk == 56 || tk == 57 || tk == 58 || tk == 224 || tk == 228;
        }
    }
}
