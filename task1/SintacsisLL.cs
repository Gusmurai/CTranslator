using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace task1
{
    internal class SintacsisLL
    {
        Lexan lexan;


        int index;
        private ListBox listBox;
        public List<Token> Tokens;
        public SintacsisLL(List<Token> tokens, ListBox listBox = null)
        {
            this.listBox = listBox;
            Tokens = tokens;
            int x = prog();

        }
      
        private void Error(int kod, int index)
        {

            string errorMessage = $"Синтаксическая ошибка #{kod} на позиции {index}: ";

            switch (kod)
            {
                case -1:
                    errorMessage = "Ошибок нет";
                    break;
                case 0:
                    errorMessage += "Ожидалось ключевое слово 'main'";
                    break;
                case 1:
                    errorMessage += "Ожидалось ключевое слово 'int'";
                    break;
                case 2:
                    errorMessage += "Ожидалось ключевое слово 'long'";
                    break;
                case 3:
                    errorMessage += "Ожидалось ключевое слово 'while'";
                    break;
                
                case 10:
                    errorMessage += "Ожидалась открывающая скобка '('";
                    break;
                case 11:
                    errorMessage += "Ожидалась закрывающая скобка ')'";
                    break;
                case 12:
                    errorMessage += "Ожидалась открывающая фигурная скобка '{'";
                    break;
                case 13:
                    errorMessage += "Ожидалась закрывающая фигурная скобка '}'";
                    break;

                case 14:
                    errorMessage += "Ожидалась точка с запятой ';'";
                    break;
                case 15:
                    errorMessage += "Ожидался арифметический оператор (*, /, %, -, +)";
                    break;
                case 16:
                    errorMessage += "Ожидался оператор инкремента '++'";
                    break;

                case 17:
                    errorMessage += "Ожидалось логическое НЕ '!'";
                    break;
                case 18:
                    errorMessage += "Ожидался оператор сравнения (>, <, >=, <=, ==, !=)";
                    break;


                case 19:
                    errorMessage += "Ожидался идентификатор";
                    break;
                case 20:
                    errorMessage += "Ожидался идентификатор или литерал";
                    break;
                case 21:
                    errorMessage += "Ожидалось ')', '||', '&&')";
                    break;

                case 22:
                    errorMessage += "Ожидалось ')' '||' '&&' '>', '<', '>=', '<=', '==', '!='";
                    break;
                case 23:
                    errorMessage += "Ожидался идентификатор или литерал или '('";
                    break;
                case 24:
                    errorMessage += "Ожидалось ';' или '{'";
                    break;
                case 25:
                    errorMessage += "Ожидались 'int', 'long', 'идентификатор', '++', 'while'";
                    break;
                case 26:
                    errorMessage += "Ожидались ';', '+', '-', '*','/', '%'";
                    break;
                case 27:
                    errorMessage += "Ожидались  ')', '||'";
                    break;

                default:
                    errorMessage += "Неизвестная синтаксическая ошибка";
                    break;
            }

            // Добавляем информацию о текущем токене
            if (index < Tokens.Count)
            {
                errorMessage += $". Обнаружено: {GetRussianTokenType(Tokens[index].Type)}";
                if (!string.IsNullOrEmpty(Tokens[index].Value))
                {
                    errorMessage += $" '{Tokens[index].Value}'";
                }
            }
            else
            {
                errorMessage += ". Достигнут конец входных данных";
            }


            // Выводим ошибку в ListBox
            if (listBox != null)
            {
                listBox.Items.Add(errorMessage);
            }
            else
            {
                Console.WriteLine(errorMessage);
            }
        }

        
        private string GetRussianTokenType(string type)
        {
            switch (type)
            {
                case "SW":
                    return "ключевое слово";
                case "R":
                    return "разделитель";
                case "I":
                    return "идентификатор";
                case "L":
                    return "литерал";
                
                default:
                    return type;
            }
        }


        public int prog()
        {
            int kod = -1;
            index = 0;

            if (!(Tokens[index].Type == "SW" && Tokens[index].Number == 0))//main
            {
                Error(0, index); return 0;
            }
            index++;

            if (!(Tokens[index].Type == "R" && Tokens[index].Number == 0))//(
            {
                Error(10, index); return 10;
            }
            index++;

            if (!(Tokens[index].Type == "R" && Tokens[index].Number == 1))//)
            {
                Error(11, index); return 11;
            }
            index++;

            if (!(Tokens[index].Type == "R" && Tokens[index].Number == 2))//{
            {
                Error(12, index); return 12;
            }
            index++;

            kod = spis_op(ref index);
            if (kod >= 0) return kod;

            if (!(Tokens[index].Type == "R" && Tokens[index].Number == 3))//}
            {
                Error(13, index); return 13;
            }
            index++;
            index++;
            Error(-1, index);
            return -1;
        }
        private int spis_op(ref int index) // <опер><x>
        {
            int kod = -1;
            kod = oper(ref index); // там ++
            if (kod >= 0)
                return kod;

            kod = X(ref index);

            return kod;
        }
        private int oper(ref int index)
        {
            int kod = -1;
            // Проверка для int id <Y>;
            if (Tokens[index].Type == "SW" && Tokens[index].Number == 1) // int
            {
                index++;
                if (!(Tokens[index].Type == "ID")) // id
                {
                    Error(19, index); return 19;
                }
                index++;

                kod = Y(ref index);
                if (kod >= 0) return kod;

                if (!(Tokens[index].Type == "R" && Tokens[index].Number == 4)) // ;
                {
                    Error(14, index); return 14;
                }
                index++;
                return -1;
            }

            // Проверка для long id <Y>;
            if (Tokens[index].Type == "SW" && Tokens[index].Number == 2) // long
            {
                index++;
                if (!(Tokens[index].Type == "ID")) // id
                {
                    Error(19, index); return 19;
                }
                index++;
                kod = Y(ref index);
                if (kod >= 0) return kod;

                if (!(Tokens[index].Type == "R" && Tokens[index].Number == 4)) // ;
                {
                    Error(14, index); return 14;
                }
                index++;
                return -1;
            }
            // Проверка для id<Q>;
            if (Tokens[index].Type == "ID") //id
            {
                index++;
                kod = Q(ref index);
                if (kod >= 0) return kod;

                if (!(Tokens[index].Type == "R" && Tokens[index].Number == 4)) // ;
                {
                    Error(14, index); return 14;
                }
                index++;
                return -1;
            }

            // Проверка для ++id;
            if (Tokens[index].Type == "R" && Tokens[index].Number == 16) // ++
            {
                index++;
                if (!(Tokens[index].Type == "ID")) // id
                {
                    Error(19, index); return 19;
                }
                index++;

                if (!(Tokens[index].Type == "R" && Tokens[index].Number == 4)) // ;
                {
                    Error(14, index); return 14;
                }
                index++;
                return -1;
            }

            // Проверка для <цикл>
            if (Tokens[index].Type == "SW" && Tokens[index].Number == 3) // while
            {
                kod = cicl(ref index);
                return kod;
            }
            Error(25, index); return 25;  // ожидали что-то из
        }

        private int X(ref int index)
        {
            int kod = -1;
            //проверка на эпсилон
            if (Tokens[index].Type == "R" && Tokens[index].Number == 3) // }
                return -1;

            kod = spis_op(ref index);
            if (kod >= 0) return kod;

            return -1;
        }

        private int Q(ref int index)
        {
            int kod = -1;
            //Проверка для =<выр>
            if (Tokens[index].Type == "R" && Tokens[index].Number == 12) // = <выр>
            {
                index++;
                kod = vir(ref index);
                 return kod;   // убрала if (kod >= 0)
            }
            if (Tokens[index].Type == "R" && Tokens[index].Number == 16) // ++
            {
                index++;
                return -1;
            }

            Error(16, index); return 16; // ожидался ++ 
        }

        private int Y(ref int index)
        {
            int kod = -1;
            //закрытие эпсилона ;
            if (Tokens[index].Type == "R" && Tokens[index].Number == 4) // ;
            
                return -1;
            
            //Проверка на = <выр>
            if (Tokens[index].Type == "R" && Tokens[index].Number == 12) // =
            {
                index++;
                kod = vir(ref index);
                if (kod >= 0) return kod;
            }
           return -1;
        }
        private int vir(ref int index) // <эл><z>
        {
            int kod = -1;
            kod = el(ref index);
            if (kod >= 0)
                return kod;

            kod = Z(ref index);
            return kod;
        }
        private int el(ref int index)
        {
            int kod = -1;

            if (!(Tokens[index].Type == "ID" || Tokens[index].Type == "L")) // id или lit
            {
                Error(20, index); return 20;
            }
            index++;

            return -1;
        }

        private int Z(ref int index)
        {
            int kod = -1;
            //Проверка на эпсилон
            if (Tokens[index].Type == "R" && Tokens[index].Number == 4) // ;
            {
                return -1;
            }
            else
            {
                kod = op(ref index);
                if (kod >= 0) return kod;

                kod = el(ref index);

                return kod;

            }

        }
        private int op(ref int index)
        {
            int kod = -1;
            if (Tokens[index].Type == "R" && Tokens[index].Number == 9) // +
            {
                index++;
                return -1;
            }
            if (Tokens[index].Type == "R" && Tokens[index].Number == 8) // -
            {
                index++;
                return -1;
            }
            if (Tokens[index].Type == "R" && Tokens[index].Number == 5) // *
            {
                index++;
                return -1;
            }
            if (Tokens[index].Type == "R" && Tokens[index].Number == 6) // /
            {
                index++;
                return -1;
            }
            if (Tokens[index].Type == "R" && Tokens[index].Number == 7) // %
            {
                index++;
                return -1;
            }

            Error(15, index); return 15; // ожидался арифметический оператор
        }
        private int cicl(ref int index)
        {
            int kod = -1;
            if (!(Tokens[index].Type == "SW" && Tokens[index].Number == 3)) // while
            {
                Error(3, index); return 3;
            }
            index++;

            if (!(Tokens[index].Type == "R" && Tokens[index].Number == 0)) // (
            {
                Error(10, index); return 10;
            }
            index++;

            kod = A(ref index);
            if (kod >= 0) return kod;


            if (!(Tokens[index].Type == "R" && Tokens[index].Number == 1)) // )
            {
                Error(11, index); return 11;
            }
            index++;

            kod = bloc(ref index);
            if (kod >= 0)  return kod;

                return -1;
        }

        private int A(ref int index)
        {
            int kod = -1;
            kod = B(ref index);
            if (kod >= 0)
                return kod;

            kod = L(ref index);
            return kod;
        }
        private int L(ref int index)
        {
            int kod = -1;
            //закрытие эпсилона ;
            if (Tokens[index].Type == "R" && Tokens[index].Number == 1) // )
                //index++;
                return -1;
            //Проверка на ||<B><L>
            if (Tokens[index].Type == "R" && Tokens[index].Number == 21) // ||
            {
                index++;
                kod = B(ref index);
                if (kod >= 0) return kod;

                kod = L(ref index);
                 return kod;
            }
            Error(27, index); return 27; // ожидалась ) или ||
        }
        private int B(ref int index)
        {
            int kod = -1;
            kod = C(ref index);
            if (kod >= 0)
                return kod;

            kod = S(ref index);
            return kod;
        }
        private int S(ref int index)
        {
            int kod = -1;
            //закрытие эпсилона
            if (Tokens[index].Type == "R" && (Tokens[index].Number == 1 || Tokens[index].Number == 21)) // ) ||
                //index++;
                return -1;

            //Проверка на &&<C><S>
            if (Tokens[index].Type == "R" && Tokens[index].Number == 22) // &&
            {
                index++;
                kod = C(ref index);
                if (kod >= 0) return kod;

                kod = S(ref index);
                 return kod;  //убираю if (kod >= 0)
            }
            Error(21, index); return 21; // ожидалась ) или || &&
        }
        private int C(ref int index)
        {
            int kod = -1;
            kod = D(ref index);
            if (kod >= 0)
                return kod;

            kod = N(ref index);
            return kod;
        }
        private int N(ref int index)
        {
            int kod = -1;
            // Закрытие эпсилона
            if (Tokens[index].Type == "R" && (Tokens[index].Number == 1 || Tokens[index].Number == 21 || Tokens[index].Number == 22)) // ) || &&

            return -1;

            
           else {// Проверка на <оп_ср><D><N>
                kod = opSrav(ref index);
                if (kod >= 0) return kod;

                kod = D(ref index);
                if (kod >= 0) return kod;

                kod = N(ref index);
                return kod;
            }

        }

        private int opSrav(ref int index)
        {
            int kod = -1;
            if (Tokens[index].Type == "R" && Tokens[index].Number == 10) // >
            {
                index++;
                return -1;
            }
            if (Tokens[index].Type == "R" && Tokens[index].Number == 11) // <
            {
                index++;
                return -1;
            }
            if (Tokens[index].Type == "R" && Tokens[index].Number == 18) // >=
            {
                index++;
                return -1;
            }
            if (Tokens[index].Type == "R" && Tokens[index].Number == 17) // <=
            {
                index++;
                return -1;
            }
            if (Tokens[index].Type == "R" && Tokens[index].Number == 19) // ==
            {
                index++;
                return -1;
            }
            if (Tokens[index].Type == "R" && Tokens[index].Number == 20) // !=
            {
                index++;
                return -1;
            }


            Error(18, index); return 18; // ожидался оператор сравнения
        }
        private int D(ref int index)
        {
            int kod = -1;
            if (Tokens[index].Type == "R" && Tokens[index].Number == 13) // !
            {
                index++;
                kod = G(ref index);
                return kod; //if (kod >= 0)

            }
            else
            {
                kod = G(ref index);
                return kod; //if (kod >= 0) 

            }

        }
        private int G(ref int index)
        {
            int kod = -1;

            if (Tokens[index].Type == "ID") // id
            {
                index++;
                return -1;
            }

            if (Tokens[index].Type == "L") // lit
            {
                index++;
                return -1;
            }

            //(<A>)
            if (Tokens[index].Type == "R" && Tokens[index].Number == 0) // (
            {
                index++;
                kod = A(ref index);
                if (kod >= 0) return kod;

                if (!(Tokens[index].Type == "R" && Tokens[index].Number == 1)) // )
                {
                    Error(1, index); return 1;
                }
                index++;
                return -1;
            }


            Error(23, index); return 23; // ожидали id lit (
        }
        private int bloc(ref int index)
        {
            int kod = -1;
            if (Tokens[index].Type == "R" && Tokens[index].Number == 4) // ;
            {
                index++;
                return -1;
            }

            else if (Tokens[index].Type == "R" && Tokens[index].Number == 2) // {
            {
                index++;
                kod = spis_op(ref index);
                if (kod >= 0) return kod;

                if (!(Tokens[index].Type == "R" && Tokens[index].Number == 3)) // }
                {
                    Error(13, index); return 13;
                }
                index++;
                return -1;
            }
            else
            {
                kod = oper(ref index);
               return kod; // if (kod >= 0) 
            }
            



        }
    }
}


    
