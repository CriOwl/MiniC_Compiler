using System;
using System.Collections.Generic;

namespace MiniC
{
    class TestConsole
    {
        public static void RunTests()
        {
            Console.WriteLine("==========================================");
            Console.WriteLine("       EJECUTANDO PRUEBAS DE CONSOLA      ");
            Console.WriteLine("==========================================");

            string[] tests = new string[]
            {
                // 1. Array Initialization
                @"
                int a[3] = {1, 2, 3};
                void main() { 
                    int x = a[0]; 
                }
                ",

                // 2. Struct and Union
                @"
                struct Point { int x; int y; };
                union Data { int i; float f; };
                void main() {
                    struct Point p;
                    p.x = 10;
                    union Data d;
                    d.i = 5;
                }
                ",

                // 3. Control Flow
                @"
                int suma(int a, int b) {
                    if (a > b) return a;
                    else return b;
                }
                void main() {
                    int i = 0;
                    while(i < 10) {
                        i = i + 1;
                        if (i == 5) continue;
                    }
                }
                ",

                // 4. Nested Loops and Switch
                @"
                void main() {
                    int i;
                    for (i=0; i<5; i=i+1) {
                       switch(i) {
                           case 1: break;
                           default: break;
                       }
                    }
                }
                ",

                // 5. General Function
                @"
                int test(int x, ...) {
                    return x;
                }
                void main() {}
                "
            };

            AnalizadorLexico lexer = new AnalizadorLexico();
            lexer.ModoConsola = true;

            AnalizadorSintactico parser = new AnalizadorSintactico();
            parser.ModoConsola = true; // Si pudiera acceder, pero es privada/helper? 
                                       // Wait, I made ModoConsola public in AnalizadorSintactico in Step 228.

            int count = 1;
            foreach (string code in tests)
            {
                Console.WriteLine($"\n--- PRUEBA {count} ---");
                Console.WriteLine(code.Trim());
                Console.WriteLine("------------------");

                try
                {
                    List<Tokens> tokens = lexer.AnalisisLexico(code);
                    // AnalisisLexico resets its state? No, Cont is global in class. 
                    // Need to recreate Lexer for each test or reset Cont.
                    // Checking code: Cont is global field. Need new instance.
                }
                catch (Exception ex) { Console.WriteLine("Lexer crash: " + ex.Message); }

                // Lexer instance reuse issue: Cont is 35. 
                // Better: new instance each time.
            }
        }

        public static void RunFullTest()
        {
            // Better implementation to handle instance reset
            string[] tests = new string[]
           {
                "int a=1; void main(){}",
                "struct S { int x; }; void main() { struct S s; s.x=1; }",
                "void main() { int i; for(i=0;i<10;i=i+1) {} }",
                "union U { int i; }; void main() {}",
                // User's failing case:
                @"
#include <stdio.h>

int main(void) {
    int n; 
    int suma = 0;

    printf(""Ingrese un numero entero positivo: "");
    scanf(""%d"", &n);

    if (n < 0) {
        printf(""El numero debe ser positivo.\n"");
        return 1;
    }

    for (int i = 1; i <= n; i++) {
        suma += i;
    }

    printf(""La suma de los numeros del 1 al %d es: %d\n"", n, suma);

    return 0;
}
",
                "void expression_test() { int a; int b; int c; int d; int e; a = b | c ^ d & e; }",
                "void complex_test() { int x; int y; int z; x = y = z + 1; }",
                "void paren_test() { int a; int b; int c; (a + b) * c; }",
                "void logic_test() { int a; int b; int c; int x; if (a && b || c) x = 1; }"
           };

            /*
            int i = 1;
            foreach (var t in tests)
            {
                Console.WriteLine($"\n[TEST {i}] Code: {t}");
                TestOne(t);
                i++;
            }
            */
            Console.WriteLine("[TEST TARGET] Character Constants");
            TestOne("void main() { char op = '+'; if (op == '+') return 1; }");
        }

        static void TestOne(string input)
        {
            try
            {
                AnalizadorLexico lex = new AnalizadorLexico();
                lex.ModoConsola = true;
                Console.WriteLine("  > Lexer start...");
                var tokens = lex.AnalisisLexico(input);
                Console.WriteLine("  > Lexer end. Tokens: " + tokens.Count);

                if (tokens.Count > 0)
                {
                    AnalizadorSintactico parser = new AnalizadorSintactico();
                    parser.ModoConsola = true;
                    Console.WriteLine("  > Parser start...");
                    parser.AnalisisSintactico(tokens);
                    Console.WriteLine("  > Parser end.");
                    if (parser.Compilacion)
                        Console.WriteLine(">> Sintaxis: OK");
                    else
                        Console.WriteLine(">> Sintaxis: FAILED");
                }
                else
                {
                    Console.WriteLine(">> Lexer vacio o error.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("!! CRITICAL ERROR: " + ex.ToString());
            }
            Console.Out.Flush();
        }
    }
}
