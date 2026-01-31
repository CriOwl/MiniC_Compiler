using System;
using System.Drawing;
using System.Collections.Generic;
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
        readonly string AD = "\n\r\a\t";
        int Cont = 0; //variable global que cuenta en que posición del archivo me encuentro
        int Linea = 1; //variable global para el numero de linea
        string Lexema = string.Empty; //vacía el lexema cada que se completa un token
        public bool ErrorLexico = false;

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
            if (c == '0') return 0;
            if (c >= '1' && c <= '7') return 1;
            if (c == '8' || c == '9') return 2;
            if (c == '+') return 3;
            if (c == '-') return 4;
            if (c == '.') return 5;
            if (c == 'e' || c == 'E') return 6;
            if (c == 'x' || c == 'X') return 7;
            if ("abcdfABCDF".Contains(c)) return 8; // Incluye 'f/F' como dígito hex y sufijo
            if (c == 'u' || c == 'U') return 9;     // Sufijo Unsigned
            if (c == 'l' || c == 'L') return 10;    // Sufijo Long
            return -1;
        }
        protected int GetAlfabetoDosSimbolos(char c)
        {
            if (c == '*')
                return 0;
            else if (c == '%')
                return 1;
            else if (c == '!')
                return 2;
            else if (c == '=')
                return 3;
            else if (c == '^')
                return 4;
            else if (c == '>')
                return 5;
            else if (c == '<')
                return 6;
            else if (c == '&')
                return 7;
            else if (c == '|')
                return 8;
            return -1; // si c no coincide con ninguno retorna -1
        }
        protected int GetAlfabetoSimboloDiagonal(char c)
        {
            if (c == '/')
                return 0;
            else if (c == '*')
                return 1;
            else if (c == '=')
                return 2;
            else if (c == '0')
                return 3;
            return -1; // si c no coincide con ninguno retorna -1
        }
        protected int GetAlfabetoDobleComilla(char c)
        {
            if (c == '"')
                return 6;
            else if (c == '%')
                return 5;
            else if (c == ' ')
                return 4;
            else if (c == '.')
                return 7;
            else if (c == '\\') // Nueva columna 13 para la barra invertida
                return 13;
            else if (c == 'd')
                return 8;
            else if (c == 'c')
                return 9;
            else if (c == 'f')
                return 10;
            else if (c == 's')
                return 11;
            else if (c == 'i')
                return 12;
            else if (L.Contains(c))
                return 0;
            else if (D.Contains(c))
                return 1;
            else if (AD.Contains(c))
                return 2;
            else if (AR.Contains(c))
                return 3;

            return -1;
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
                // --- LÓGICA PARA RECONOCIMIENTO C.1 ---

                // 1. Si el lexema tiene longitud 1, buscamos su token de letra individual
                if (Lexema.Length == 1)
                {
                    // GetTokenLetra buscará en tu diccionario de UnidadesLexicas
                    // y encontrará que "a" es 400, "b" es 401, etc.
                    int tokenIndividual = UL.GetTokenLetra(Lexema);
                    LstTokens.Add(new Tokens(Linea, Lexema, tokenIndividual));
                }
                else
                {
                    // 2. Si tiene más de una letra, verificamos si es Palabra Reservada (int, void, etc.)
                    // o si se queda como Identificador (300)
                    int tokenPalabra = UL.GetTokenPalabra(Lexema);
                    LstTokens.Add(new Tokens(Linea, Lexema, tokenPalabra));
                }

                Lexema = string.Empty;
            }
        }

        protected void AutomataNumeros(string Archivo)
        {
            char c;
            int Estado = 0;
            int Simbolo;
            string Lexema = string.Empty;

            // TT: Estados x 11 Columnas (0-10)
            // Col: 0:0 | 1:1-7 | 2:8-9 | 3:+ | 4:- | 5:. | 6:eE | 7:xX | 8:hex/fF | 9:uU | 10:lL
            int[,] TT =
            {
            /* 0: Inicio    */ {11,  5,  5,  1,  2,  4, -1, -1, -1, -1, -1},
            /* 1: +         */ { 5,  5,  5, 15, 15,  4, 15, 15, 15, 15, 15},
            /* 2: -         */ { 5,  5,  5, 15, 15,  4, 15, 15, 15, 15, 15},
            /* 3: (Acep Op) */ {-1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
            /* 4: . inicial */ { 7,  7,  7, 15, 15, 15, 15, 15, 15, 15, 15},
            /* 5: Entero    */ { 5,  5,  5, 15, 15,  6,  8, 15, 15, 16, 17}, // Puede ir a sufijos
            /* 6: . tras dig*/ { 7,  7,  7, 15, 15, 15,  8, 15, 15, 15, 15},
            /* 7: Flotante  */ { 7,  7,  7, 15, 15, 15,  8, 15, 18, 15, 18}, // f, F o L
            /* 8: Exponente */ {10, 10, 10,  9,  9, 15, 15, 15, 15, 15, 15},
            /* 9: Signo Exp */ {10, 10, 10, 15, 15, 15, 15, 15, 15, 15, 15},
            /* 10: Digitos E*/ {10, 10, 10, 15, 15, 15, 15, 15, 18, 15, 18}, // f, F o L
            /* 11: Leading 0*/ {14, 14, 15, 15, 15,  6,  8, 12, 15, 16, 17},
            /* 12: Start 0x */ {13, 13, 13, 15, 15, 15, 15, 15, 13, 16, 17},
            /* 13: Hex Seq  */ {13, 13, 13, 15, 15, 15, 15, 15, 13, 16, 17},
            /* 14: Octal Seq*/ {14, 14, 15, 15, 15, 15, 15, 15, 15, 16, 17},
            /* 15: ERROR    */ {15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15},
            /* 16: Sufijo U */ {15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 17}, // Tras U puede venir L
            /* 17: Sufijo L */ {15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 17}, // Tras L puede venir otra L (LL)
            /* 18: Sufijo F */ {15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15}  // Estado final para f/F/L en flotante
            };

            do
            {
                c = Archivo[Cont];
                Simbolo = GetAlfabetoNumero(c);
                if (Simbolo == -1) break;

                int ProximoEstado = TT[Estado, Simbolo];
                if (ProximoEstado == -1) break;

                Estado = ProximoEstado;
                Lexema += c;
                Cont++;
            } while (Cont < Archivo.Length);

            // --- CLASIFICACIÓN CON SUFIJOS ---
            if (Estado == 15)
            {
                //MessageBox.Show($"Error léxico: '{Lexema}' mal formado.");
                ErrorLexico = true;
            }
            else if (Estado == 13) LstTokens.Add(new Tokens(Linea, Lexema, 305)); // Hexa
            else if (Estado == 14) LstTokens.Add(new Tokens(Linea, Lexema, 304)); // Octal
            else if (new[] { 5, 11, 16, 17 }.Contains(Estado)) LstTokens.Add(new Tokens(Linea, Lexema, 301)); // Entero
            else if (new[] { 7, 10, 18 }.Contains(Estado)) LstTokens.Add(new Tokens(Linea, Lexema, 302)); // Flotante
            else if (Lexema == "+" || Lexema == "-") LstTokens.Add(new Tokens(Linea, Lexema, UL.GetTokenSimbolo(Lexema)));

            Lexema = string.Empty;
        }

        protected void AutomataDosSimbolos(string Archivo)
        {
            char c;
            int Estado = 0; //estado de trancisión en el que nos encontramos
            int Simbolo; // representa el simbolo = mediante el indice de la columna de TT en la que nos encontramos
            string Lexema = string.Empty; // contiene el lexema pero se vacia antes de iniciar
            int[,] TT =
            {
                { 1,  2, 3,  4,  5,  6,  7,  8,  9},//0
                {-1, -1, 1, 11, -1, -1, -1, -1, -1},//1
                {-1, -1, 1, 11, -1, -1, -1, -1, -1},//2
                {-1, -1, 1, 11, -1, -1, -1, -1, -1},//3
                {-1, -1, 1, 11, -1, -1, -1, -1, -1},//4
                {-1, -1, 1, 11, -1, -1, -1, -1, -1},//5
                {-1, -1, 1, 11, -1, 12, -1, -1, -1},//6
                {-1, -1, 1, 11, -1, -1, 13, -1, -1},//7
                {-1, -1, 1, 11, -1, -1, -1, 14, -1},//8
                {-1, -1, 1, 11, -1, -1, -1, -1, 15},//9
                {-1, -1, 1, -1, -1, -1, -1, -1, -1},//10
                {-1, -1, 1, -1, -1, -1, -1, -1, -1},//11
                {-1, -1, 1, -1, -1, -1, -1, -1, -1},//12
                {-1, -1, 1, -1, -1, -1, -1, -1, -1},//13
                {-1, -1, 1, -1, -1, -1, -1, -1, -1}//14
            };
            do
            {
                c = Archivo[Cont]; // recibimo el simbolo en posicion Cont
                Simbolo = GetAlfabetoDosSimbolos(c); // esta deberia reconocer que simbolo es y retornar el indice de la columna
                if (Simbolo == -1)
                    break; // si caemos en estado de rechazo termina ejecucion del automata
                Lexema += c; // si es un caracter reconocible por el automata lo añadimos al lexema
                Estado = TT[Estado, Simbolo]; // ejecutamos el automata (es decir actualizamos el estado acorde con lo que diga la tabla)
                Cont++;
            } while (Cont < Archivo.Length);
            if (new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14 }.Contains(Estado)) // si el ultimo estado en el que se quedo el automata es de aceptacion guarda en lista de tokens
            {
                LstTokens.Add(new Tokens(Linea, Lexema, UL.GetTokenSimbolo(Lexema)));
                Lexema = string.Empty;
            }
        }

        protected void AutomataSimboloDiagonal(string Archivo)
        {
            char c;
            int Estado = 0; //estado de trancisión en el que nos encontramos
            int Simbolo; // representa el simbolo = mediante el indice de la columna de TT en la que nos encontramos
            string Lexema = string.Empty; // contiene el lexema pero se vacia antes de iniciar
            int[,] TT =
            {
                { 1,  -1, -1,  -1},//0
                { 5,   2,  6,  -1},//1
                { 2,   3,  2,   2},//2
                { 4,   3,  2,   2},//3
                {-1,  -1, -1,  -1},//4
                { 5,   5,  5,   5},//5
                {-1,  -1, -1,  -1}//6
            };
            do
            {
                c = Archivo[Cont]; // recibimo el simbolo en posicion Cont
                Simbolo = GetAlfabetoSimboloDiagonal(c); // esta deberia reconocer que simbolo es y retornar el indice de la columna
                if (Simbolo == -1)
                    break; // si caemos en estado de rechazo termina ejecucion del automata
                Lexema += c; // si es un caracter reconocible por el automata lo añadimos al lexema
                Estado = TT[Estado, Simbolo]; // ejecutamos el automata (es decir actualizamos el estado acorde con lo que diga la tabla)
                Cont++;
            } while (Cont < Archivo.Length);
            if (new[] { 1, 4, 5, 6 }.Contains(Estado)) // si el ultimo estado en el que se quedo el automata es de aceptacion guarda en lista de tokens
            {
                LstTokens.Add(new Tokens(Linea, Lexema, UL.GetTokenSimbolo(Lexema)));
                Lexema = string.Empty;
            }
        }

        protected void AutomataDobleComilla(string Archivo)
        {
            int Estado = 0;
            int Simbolo;
            string Lexema = string.Empty;

            int[,] TT =
            {
                {-1, -1, -1, -1, -1, -1,  1, -1, -1, -1, -1, -1, -1, -1}, // 0
                { 5,  5,  5,  5,  5,  3,  2,  5,  5,  5,  5,  5,  5,  8}, // 1
                {-1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1}, // 2
                {-1,  3, -1, -1, -1,  5, -1,  6,  4,  4,  4,  4,  4, -1}, // 3
                { 5,  5,  5,  5,  5,  3,  2,  5,  5,  5,  5,  5,  5,  8}, // 4
                { 5,  5,  5,  5,  5,  3,  2,  5,  5,  5,  5,  5,  5,  8}, // 5
                {-1,  7, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1}, // 6
                {-1,  7, -1, -1, -1, -1, -1, -1, -1, -1,  4, -1, -1, -1}, // 7
                { 5, -1,  5, -1, -1, -1, -1, -1,  5,  5,  5,  5,  5, -1}  // 8
            };

            do
            {
                char c = Archivo[Cont];
                Simbolo = GetAlfabetoDobleComilla(c);

                if (Simbolo == -1) break;

                int proximoEstado = TT[Estado, Simbolo];
                if (proximoEstado == -1) break;

                Estado = proximoEstado;
                Lexema += c;
                Cont++;

                if (Estado == 2) break;

            } while (Cont < Archivo.Length);

            if (Estado == 2)
            {
                LstTokens.Add(new Tokens(Linea, Lexema, 303));
            }
            else
            {
                Console.WriteLine("Error léxico: Cadena de texto mal formada.");
            }
        }

        protected void AutomataCaracter(string Archivo)
        {
            // Esperamos: ' <cualquier_cosa> '
            string lexemaChar = "'";
            Cont++; // Saltamos la primera comilla '

            if (Cont < Archivo.Length)
            {
                char c = Archivo[Cont];
                lexemaChar += c; // Este es el "CUALQUIER_CARACTER"
                Cont++;

                if (Cont < Archivo.Length && Archivo[Cont] == '\'')
                {
                    lexemaChar += "'";
                    Cont++;
                    // Token 306: Constante de Carácter
                    LstTokens.Add(new Tokens(Linea, lexemaChar, 306));
                }
                else
                {
                    MessageBox.Show("Error léxico: Constante de carácter mal cerrada.");
                    ErrorLexico = true;
                }
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
                else if ("\t\0 \a".Contains(c))
                {
                    Cont++;
                }
                //Autómatas de un solo símbolo
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
                else if (D.Contains(c) || "+-.".Contains(c))
                {
                    AutomataNumeros(Archivo);
                }

                //Automatas de dos simbolos
                else if ("*%!=^><&|".Contains(c))
                {
                    AutomataDosSimbolos(Archivo);
                }

                //Automata de Simbolo Diagonal
                else if (c == '/')
                {
                    // Miramos el siguiente caracter sin mover el contador global todavía
                    if (Cont + 1 < Archivo.Length && Archivo[Cont + 1] == '/')
                    {
                        // Es un comentario de una línea, avanzamos hasta el final de la línea
                        while (Cont < Archivo.Length && Archivo[Cont] != '\n')
                        {
                            Cont++;
                        }
                    }
                    else if (Cont + 1 < Archivo.Length && Archivo[Cont + 1] == '*')
                    {
                        // Es un comentario multilínea /* ... */
                        Cont += 2; // Saltamos el /*
                        while (Cont + 1 < Archivo.Length && !(Archivo[Cont] == '*' && Archivo[Cont + 1] == '/'))
                        {
                            if (Archivo[Cont] == '\n') Linea++;
                            Cont++;
                        }
                        Cont += 2; // Saltamos el */
                    }
                    else
                    {
                        // Si no es // ni /*, entonces es el operador de división
                        AutomataSimboloDiagonal(Archivo);
                    }
                }

                //Automata de Doble Comilla
                else if ("\"".Contains(c))
                {
                    AutomataDobleComilla(Archivo);
                }

                else if (c == '\'')
                {
                    AutomataCaracter(Archivo);
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

        protected bool Parea(int Tk, string Lexema) //recibe el num token y el lexema y verifica si es el token esperado
        {
            if (ListaTokens[Cont].Token == Tk)
            {
                Cont++;
                return true;
            }
            else
            {
                MessageBox.Show("Error sintáctico: Se esperaba " + Lexema + " en la línea " + ListaTokens[Cont].Linea + ".");
                Compilacion = false;
                return false;
            }
        }

        #region C.1 Elementos básicos de la gramática

        protected void gramatica_digito_decimal()
        {
            if (Parea(301, "Número decimal")) return;
        }

        protected void gramatica_digito_octal()
        {
            if (Parea(304, "Número octal")) return;
        }
        protected void gramatica_digito_hexadecimal()
        {
            if (Parea(305, "Número hexadecimal")) return;
        }

        protected void gramatica_letra()
        {
            int tk = ListaTokens[Cont].Token;
            if ((tk >= 350 && tk <= 375) || (tk >= 400 && tk <= 425))
            {
                Cont++;
            }
            else
            {
                Parea(400, "Letra (a-z, A-Z)");
            }
        }
        protected void gramatica_idetificador_o_nombre_de_tipo()
        {
            if (Parea(300, "Identificador o Nombre de Tipo")) return;
        }

        protected void gramatica_constante()
        {
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
                    // (No mandamos error aquí todavía porque gramatica_constante 
                    // suele ser parte de una regla mayor)
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

        #endregion

        #region C.2 Expresiones
        // expresión primaria
        // Foto: IDENTIFICADOR | CONSTANTE | STRING | ( expresión )

        protected void gramatica_expresion_primaria()
        {
            int tk = ListaTokens[Cont].Token;

            // IDENTIFICADOR | NOMBRE_DE_TIPO
            if (tk == 300)
            {
                gramatica_idetificador_o_nombre_de_tipo();
            }
            // CONSTANTE (Cualquiera de las de la sección C.1 que ya implementaste)
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
                gramatica_expresion();
                Parea(76, ")"); // Token 76 es ')'
            }
            else
            {
                MessageBox.Show("Error sintáctico: Se esperaba una expresión primaria en la línea " + ListaTokens[Cont].Linea);
                Compilacion = false;
            }
        }

        // expresión posfija
        // Foto: [], (), ., ->, ++, --

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
                        //AGREGAR gramatica_expresion();
                        Parea(78, "]");
                        break;

                    case 75: // (
                        Parea(75, "(");
                        if (ListaTokens[Cont].Token != 76)
                        { // Si hay argumentos
                            //AGREGAR gramatica_expresion_asignacion();
                            while (ListaTokens[Cont].Token == 91)
                            { // ,
                                Parea(91, ",");
                                //AGREGA gramatica_expresion_asignacion();
                            }
                        }
                        Parea(76, ")");
                        break;

                    case 90: // .
                        Parea(90, ".");
                        gramatica_idetificador_o_nombre_de_tipo();
                        break;

                    case 96: // ->
                        Parea(96, "->");
                        gramatica_idetificador_o_nombre_de_tipo();
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

        protected void gramatica_expresion_unaria()
        {
            int tk = ListaTokens[Cont].Token;
            // ++ o -- prefijo
            if (tk == 51 || tk == 52)
            {
                Cont++;
                gramatica_expresion_unaria();
            }
            // Operadores unarios: & (94), * (42), + (40), - (41), ! (72)
            else if (tk == 94 || tk == 42 || tk == 40 || tk == 41 || tk == 72)
            {
                Cont++;
                gramatica_expresion_cast();
            }
            // sizeof (23)
            else if (tk == 23)
            {
                Parea(23, "sizeof");
                if (ListaTokens[Cont].Token == 75)
                { // sizeof(tipo) o sizeof(exp)
                    int proximoTk = ListaTokens[Cont + 1].Token;
                    // Si el siguiente token es un tipo (int=17, char=4, float=13, long=18, etc)
                    if (proximoTk == 17 || proximoTk == 4 || proximoTk == 13 || proximoTk == 18 || proximoTk == 30)
                    {
                        Parea(75, "(");
                        Cont++; // Consume el tipo
                        Parea(76, ")");
                    }
                }
                else
                {
                    gramatica_expresion_unaria();
                }
            }
            else
            {
                gramatica_expresion_posfija();
            }
        }

        protected void gramatica_expresion_cast()
        {
            // Verifica si es un inicio de Cast: ( tipo )
            if (ListaTokens[Cont].Token == 75)
            { // (
                int proximoTk = ListaTokens[Cont + 1].Token;
                // Si el siguiente token es un tipo (int=17, char=4, float=13, long=18, double = 9, short = 21)
                if (proximoTk == 17 || proximoTk == 4 || proximoTk == 13 || proximoTk == 18 || proximoTk == 30 || proximoTk == 9 || proximoTk == 21)
                {
                    Parea(75, "(");
                    Cont++; // Consume el tipo
                    Parea(76, ")");
                    gramatica_expresion_cast();
                    return;
                }
            }
            gramatica_expresion_unaria();
        }

        // Multiplicación, División y Módulo (*, /, %)
        protected void gramatica_expresion_multiplicativa()
        {
            gramatica_expresion_cast();

            // Mientras el token actual sea un operador multiplicativo
            while (Compilacion && (ListaTokens[Cont].Token == 42 || ListaTokens[Cont].Token == 43 || ListaTokens[Cont].Token == 44))
            {
                Cont++; // Consumimos *, / o %
                gramatica_expresion_cast();
            }
        }

        // Suma y Resta (+, -)
        protected void gramatica_expresion_aditiva()
        {
            gramatica_expresion_multiplicativa();

            // Mientras el token sea + o -
            while (Compilacion && (ListaTokens[Cont].Token == 40 || ListaTokens[Cont].Token == 41))
            {
                Cont++; // Consumimos + o -
                gramatica_expresion_multiplicativa();
            }
        }

        protected void gramatica_expresion_desplazamiento()
        {
            gramatica_expresion_aditiva();
            while (ListaTokens[Cont].Token == 97 || ListaTokens[Cont].Token == 98)
            {
                Cont++; // Consume << o >>
                gramatica_expresion_aditiva();
            }
        }
        #endregion

        protected void gramatica_bloque_sentencias()
        {
            if (Parea(79, "{"))
            {
                Token = ListaTokens[Cont].Token;
                switch (Token)
                {
                    case 17: Cont++; gramatica_idetificador_o_nombre_de_tipo(); break; // para int
                    case 4: Cont++; gramatica_idetificador_o_nombre_de_tipo(); break; // para char
                    case 18: Cont++; gramatica_idetificador_o_nombre_de_tipo(); break; // para el long
                }
                if (Parea(80, "}"))
                    return;
            }

        }
        protected void gramatica_main()
        {
            //Reconoce la estructura sintactica: void main ( ) { }
            if (Parea(220, "main"))
            {
                if (Parea(75, "("))
                {
                    if (Parea(76, ")"))
                    {
                        gramatica_bloque_sentencias();
                    }
                }
            }
        }

        public void AnalisisSintactico(List<Tokens> Lista)
        {
            ListaTokens = Lista;
            Cont = 0;
            Compilacion = true;

            while (Cont < ListaTokens.Count && Compilacion)
            {
                Token = ListaTokens[Cont].Token;

                switch (Token)
                {
                    case 30:
                        Cont++; // Saltamos 'void' (prefijo)
                        gramatica_main();
                        break;
                    //PRUEBA DE CONSTANTES
                    case 50:
                        Cont++; // Saltamos '=' (prefijo)
                        gramatica_constante();
                        break;
                    case 300:
                        gramatica_idetificador_o_nombre_de_tipo();
                        break;
                    // CUANDO SE AGREGUE TODAS LA GRAMATICAS DE EXPRESION, DESCOMENTAR ESTO:
                    //case 300: // Si empieza con un identificador, podría ser una asignación
                    //    gramatica_expresion();
                    //    Parea(93, ";"); // Esperamos el ';' al final de la sentencia
                    //    break;
                    default:
                        // Si el token no coincide con nada de lo que estamos probando,
                        // avanzamos para evitar un bucle infinito
                        Cont++;
                        break;
                }
            }
        }
    }
}
