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
        public int Linea { get => _Linea; set => _Linea = value;}
        public string Lexema { get => _Lexema; set => _Lexema = value;}
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
            else if (Estado == 3) {
                LstTokens.Add(new Tokens(Linea, Lexema, UL.GetTokenSimbolo(Lexema)));
                Lexema = string.Empty;
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
                    Linea++;Cont++;
                }
                else if ("\t\0 ".Contains(c))
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
                else if ( D.Contains(c) || "+-.".Contains(c))
                {
                    AutomataNumeros(Archivo);
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
        bool Compilacion = true;

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
                return false;
            }
        }

        protected void gramatica_declaracion_variable()
        {
            if (Parea(300, "Identificador"))
                if (Parea(93, ";"))
                            return;
        }
        protected void gramatica_bloque_sentencias()
        {
            if (Parea(79, "{"))
            {
                Token = ListaTokens[Cont].Token;
                switch(Token){
                    case 17: Cont++; gramatica_declaracion_variable(); break; // para int
                    case 4: Cont++; gramatica_declaracion_variable(); break; // para char
                    case 18: Cont++; gramatica_declaracion_variable(); break; // para el long
                }
                if (Parea(80, "}"))
                    return;
            }
                
        }
        protected void gramatica_main()
        {
            // reconoce la estructura sintactica void main() {}
            if (Parea(220, "main"))
                if (Parea(75, "("))
                    if (Parea(76, ")"))
                        gramatica_bloque_sentencias();

        }

        public void AnalisisSintactico(List<Tokens> Lista)
        {
            ListaTokens = Lista;
            while (Cont < Lista.Count && Compilacion)
            {
                Token = Lista[Cont].Token;
                switch (Token)
                {
                    case 30: Cont++; gramatica_main(); break; // case 30 se refiere al token void que deberia estar seguido de main
                }
            }
        }
    }
}

