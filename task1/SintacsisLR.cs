using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace task1
{
    // Представляет четвёрку: (Оператор, Операнд1, Операнд2, Результат)
    // Используется для генерации промежуточного кода, особенно для логических выражений
    public class Quartet
    {
        public string Operator { get; set; }
        public string Operand1 { get; set; }
        public string Operand2 { get; set; }
        public string Result { get; set; }

        public Quartet(string op, string o1, string o2, string res)
        {
            Operator = op;
            Operand1 = o1;
            Operand2 = o2;
            Result = res;
        }

        public override string ToString()
        {

            return $"({Operator}, {Operand1}, {Operand2}, {Result})";
        }
    }

    // Пользовательская реализация обобщенного стека
    public class Stack<T>
    {
        // Массив для хранения элементов стека.
        public T[] mas;

        // Индекс верхнего элемента стека. -1 означает, что стек пуст
        public int top;

        // Конструктор стека.
        public Stack()
        {
            // Инициализация массива
            mas = new T[100];
            // Установка начального значения top в -1 (стек пуст)
            top = -1;
        }

        // Метод для добавления элемента в стек.
        public void Push(T a)
        {
            // Проверка, что стек не переполнен.
            if (top < mas.Length - 1)
            {
                // Увеличение top на 1 и добавление элемента a в массив
                mas[++top] = a;
            }
            else
            {
                // Выброс исключения, если стек переполнен
                throw new InvalidOperationException("Ошибка: переполнение стека. Невозможно добавить элемент в заполненный стек.");
            }
        }

        // Метод для просмотра верхнего элемента без его извлечения
        public T Read()
        {
            // Проверка, что стек не пуст
            if (top >= 0)
            {
                // Возврат верхнего элемента
                return mas[top];
            }
            // Возврат значения по умолчанию для типа T, если стек пуст (например, null для ссылочных типов, 0 для int)
            return default(T);
        }

        // Метод для извлечения и возврата верхнего элемента
        public T Pop()
        {
            // Проверка, что стек не пуст
            if (top >= 0)
            {
                // Сохранение верхнего элемента
                T item = mas[top];
                // Очистка ячейки массива (установка значения по умолчанию) и уменьшение top на 1
                mas[top--] = default(T);
                // Возврат сохраненного элемента
                return item;
            }
            // Выброс исключения, если стек пуст
            throw new InvalidOperationException("Ошибка: попытка извлечения из пустого стека.");
        }

        // Метод для извлечения нескольких элементов из стека
        // Возвращает новый верхний элемент или значение по умолчанию, если стек стал пустым
        public T Pop(int count)
        {
            // Уменьшение top на количество извлекаемых элементов
            top -= count;

            // Проверка, что стек не опустел ниже допустимого
            if (top >= -1)
            {
                // Если стек не пуст, вернуть новый верхний элемент, иначе — значение по умолчанию
                return (top >= 0) ? mas[top] : default(T);
            }
            else
            {
                // Сброс top в состояние пустого стека
                top = -1;
                // Выброс исключения, если попытка извлечь больше элементов, чем есть в стеке
                throw new InvalidOperationException("Ошибка: попытка извлечения из пустого стека или слишком много элементов для извлечения.");
            }
        }

        // Метод для проверки, пуст ли стек
        public bool isEmpty()
        {
            // Возвращает true, если top меньше 0 (стек пуст), иначе — false
            return top < 0;
        }

        // Метод для проверки, заполнен ли стек
        public bool isFull()
        {
            // Возвращает true, если top равен последнему индексу массива (стек полон), иначе — false
            return top >= mas.Length - 1;
        }
    }


    public class SintacsisLR
    {
        private ListBox listBox4; // Для вывода генерируемого кода
        private int labelCounter = 0;     // Счетчик для уникальных меток
        private Stack<string> loopLabelStack; // Стек для хранения меток циклов (условие, тело, конец)
        private int tempVarCounterArithmetic = 0; // Счетчик для временных арифметических переменных (T1, T2...)


        int iden = 0; // Счетчик объявленных идентификаторов
        int[] masType = new int[100]; // Хранит типы идентификаторов (1 для int, 2 для long, -1 для неопределенного). Индексируется позицией идентификатора в maS
        bool[] masInitialized = new bool[100]; // Хранит статус инициализации идентификаторов (true - инициализирован, false
        List<string> maS = Lexan.Identifiers; // Список имен идентификаторов, полученный из лексического анализатора

        Stack<int> StackSost;     // Стек состояний LR-анализатора: хранит номера состояний
        Stack<Token> StackRazbor; // Стек разбора (символьный стек) LR-анализатора: хранит терминалы и нетерминалы (как объекты Token)
        public List<Token> Tokens; // Входной поток токенов от лексера

        int index; // Текущий индекс в списке Tokens (указатель на следующий входной токен)
        private ListBox listBox;  // UI-элемент для отображения синтаксических ошибок
        private ListBox listBox2; // UI-элемент для отображения семантических ошибок

        private List<Quartet> logicalMatrix; // Хранит сгенерированные квадруплы для логических выражений
        private int matrixTempCounter; // Счетчик для генерации временных имен переменных (M1, M2, ...)

        public SintacsisLR(List<Token> tokens, ListBox listBox = null, ListBox listBox2 = null, ListBox listBox4 = null)
        {
            StackSost = new Stack<int>();
            StackRazbor = new Stack<Token>();
            index = 0;
            this.listBox = listBox;
            this.listBox2 = listBox2;
            Tokens = tokens;

            logicalMatrix = new List<Quartet>();
            matrixTempCounter = 0;


            this.listBox4 = listBox4;
            this.loopLabelStack = new Stack<string>(); // Инициализация стека меток
            // Инициализировать массив типов значением -1 (тип не определен)
            for (int i = 0; i < masType.Length; i++)
            {
                masType[i] = -1;
                masInitialized[i] = false; // Инициализация массива статусов инициализации значением false
            }

            StackSost.Push(0); // Поместить начальное состояние 0 в стек состояний

           

            int x = SinALR(); // Запустить процесс синтаксического анализа
        }
        // Вспомогательный метод для генерации кода
        private void GenerateCode(string line)
        {
            if (listBox4 != null)
            {
                listBox4.Items.Add(line);
            }
            
        }
        // Генерация уникальной метки
        private string GetNextLabelPrefix(string baseName)
        {
            return $"{baseName}_{++labelCounter}";
        }

        // Генерация имени для временной арифметической переменной
        private string NewArithmeticTemp()
        {
            return "T_ARITH" + (++tempVarCounterArithmetic);
        }

        // Вспомогательный метод для получения русского типа
        private string GetRussianType(string typeKeywordOrVarName)
        {
            if (typeKeywordOrVarName == "int") return "ЦЕЛОЕ";
            if (typeKeywordOrVarName == "long") return "ДЛИННОЕ_ЦЕЛОЕ";

            // Попытка определить тип по имени переменной (если это не ключевое слово)
            int varIdx = maS.IndexOf(typeKeywordOrVarName);
            if (varIdx != -1 && varIdx < masType.Length)
            {
                if (masType[varIdx] == 1) return "ЦЕЛОЕ";
                if (masType[varIdx] == 2) return "ДЛИННОЕ_ЦЕЛОЕ";
            }
            if (char.IsDigit(typeKeywordOrVarName[0]) || (typeKeywordOrVarName.Length > 1 && typeKeywordOrVarName[0] == '-' && char.IsDigit(typeKeywordOrVarName[1])))
                return "ЦЕЛОЕ_ЛИТЕРАЛ";

            return "НЕИЗВЕСТНЫЙ_ТИП";
        }

        // Вспомогательный метод для неявного приведения типов при арифметических операциях
        private void GenerateImplicitTypeConversion(string op1, string op2, string currentTempResultVar)
        {
            string typeOp1 = GetRussianType(op1);
            string typeOp2 = GetRussianType(op2);

            // Нормализуем литералы к ЦЕЛОЕ для сравнения типов
            if (typeOp1 == "ЦЕЛОЕ_ЛИТЕРАЛ") typeOp1 = "ЦЕЛОЕ";
            if (typeOp2 == "ЦЕЛОЕ_ЛИТЕРАЛ") typeOp2 = "ЦЕЛОЕ";

            if (typeOp1 == "ЦЕЛОЕ" && typeOp2 == "ДЛИННОЕ_ЦЕЛОЕ")
            {
                GenerateCode($"НЕЯВНОЕ_ПРИВЕДЕНИЕ_ТИПА: {op1} ({typeOp1}) К ТИПУ_МАКСИМАЛЬНОМУ ({typeOp2})");
                // Здесь могла бы быть генерация кода для реального приведения в ассемблере,
                // но для русского языка достаточно упоминания. Результат будет ДЛИННОЕ_ЦЕЛОЕ.
            }
            else if (typeOp1 == "ДЛИННОЕ_ЦЕЛОЕ" && typeOp2 == "ЦЕЛОЕ")
            {
                GenerateCode($"НЕЯВНОЕ_ПРИВЕДЕНИЕ_ТИПА: {op2} ({typeOp2}) К ТИПУ_МАКСИМАЛЬНОМУ ({typeOp1})");
                // Результат будет ДЛИННОЕ_ЦЕЛОЕ.
            }
        }

        // Вспомогательный метод для преобразования целочисленного условия в логическое
        private string EnsureBooleanCondition(string conditionOperand)
        {
            string typeCond = GetRussianType(conditionOperand);
            if (typeCond == "ЦЕЛОЕ" || typeCond == "ДЛИННОЕ_ЦЕЛОЕ" || typeCond == "ЦЕЛОЕ_ЛИТЕРАЛ")
            {
                string tempBoolVar = NewMatrixTemp(); // Используем тот же генератор, что и для логических временных
                GenerateCode($"ПРЕОБРАЗОВАТЬ_ЦЕЛОЕ_В_ЛОГИЧЕСКОЕ: {conditionOperand} (0=ЛОЖЬ, !=0=ИСТИНА), РЕЗУЛЬТАТ В {tempBoolVar}");
                return tempBoolVar;
            }
            return conditionOperand; // Уже логический (предположительно M_переменная)
        }


        // Генерирует новое уникальное имя временной переменной для промежуточных результатов (например, M1, M2)
        private string NewMatrixTemp()
        {
            return "M" + (++matrixTempCounter);
        }

        // Возвращает список сгенерированных четвёрок
        public List<Quartet> GetLogicalMatrix()
        {
            return logicalMatrix;
        }

        public void Sdvig()
        {
            Token currentToken = Tokens[index++]; // Получить текущий токен и продвинуть указатель входного потока
            // Присвоить SemanticValue из Value, если это ID, литерал или оператор, который имеет значение для семантического анализа, и SemanticValue еще не установлен

            if (string.IsNullOrEmpty(currentToken.SemanticValue) &&
                (currentToken.Type == "I" || currentToken.Type == "L" ||
                 (currentToken.Type == "R" && (currentToken.Value == "!" || currentToken.Value == "&&" || currentToken.Value == "||" || currentToken.Value == "<" || currentToken.Value == ">" || currentToken.Value == "<=" || currentToken.Value == ">=" || currentToken.Value == "==" || currentToken.Value == "!="))))
            {
                currentToken.SemanticValue = currentToken.Value;
            }
            StackRazbor.Push(currentToken); // Поместить токен в стек разбора
        }

        // Операция перехода (Goto)
        public void Perehod(int perehod)
        {
            StackSost.Push(perehod); // Поместить новое состояние в стек состояний
        }

        // Операция свертки в LR-анализе. Применяет правило грамматики и выполняет семантические действия
        public void Privedenie(int delete, N name) // 'delete': количество символов в правой части правила, 'name': нетерминал в левой части
        {
            Token[] rhsTokens = new Token[delete]; // Хранить токены правой части правила, извлеченные из стека
            for (int i = 0; i < delete; i++)
            {
                // Извлечь токены, составляющие правую часть продукции
                // Ожидается, что они находятся на определенных позициях относительно вершины StackRazbor
                if (StackRazbor.top - delete + 1 + i >= 0 && StackRazbor.top - delete + 1 + i <= StackRazbor.top)
                {
                    rhsTokens[i] = StackRazbor.mas[StackRazbor.top - delete + 1 + i];
                }
                else
                {
                    ErrorSin(101, index, StackSost.Read()); // Ошибка, если ожидаемые токены не найдены
                    return; // Предотвратить дальнейшую обработку при критической ошибке
                }
            }

            string resultingSemanticValue = null; // Семантическое значение для нового нетерминала

            // Семантические действия, основанные на применяемом правиле грамматики
            // Здесь генерируется промежуточный код (четвёрки) для логических выражений
            switch (name)
            {
                case N.el: // el -> I | L
                    if (delete == 1 && (rhsTokens[0].Type == "I" || rhsTokens[0].Type == "L"))
                    {
                        resultingSemanticValue = rhsTokens[0].Value; // Семантическое значение - имя идентификатора или значение литерала
                    }
                    break;

                case N.G: // G -> ( A ) | el
                    if (delete == 3 && rhsTokens[0].Value == "(" && rhsTokens[2].Value == ")") // G -> (A)
                    {
                        resultingSemanticValue = rhsTokens[1].SemanticValue; // Передать семантическое значение A
                    }
                    else if (delete == 1 && rhsTokens[0].Type == "N" && rhsTokens[0].Number == (int)N.el) // G -> el
                    {
                        resultingSemanticValue = rhsTokens[0].SemanticValue; // Передать семантическое значение el
                    }
                    break;

                case N.D: // D -> ! G | G
                    if (delete == 2 && rhsTokens[0].Value == "!") // D -> !G
                    {
                        string operand = rhsTokens[1].SemanticValue;
                        resultingSemanticValue = NewMatrixTemp(); // Результат сохраняется во временной переменной
                        logicalMatrix.Add(new Quartet("!", operand, "", resultingSemanticValue));
                    }
                    else if (delete == 1 && rhsTokens[0].Type == "N" && rhsTokens[0].Number == (int)N.G) // D -> G
                    {
                        resultingSemanticValue = rhsTokens[0].SemanticValue; // Передать семантическое значение G
                    }
                    break;

                case N.op_sr: // op_sr -> < | > | <= | >= | == | != (оператор сравнения)
                    if (delete == 1 && rhsTokens[0].Type == "R")
                    {
                        resultingSemanticValue = rhsTokens[0].Value; // Семантическое значение - сам оператор
                    }
                    break;

                case N.C: // C -> D op_sr D | D
                    if (delete == 3 && rhsTokens[1].Type == "N" && rhsTokens[1].Number == (int)N.op_sr) // C ->  C op_sr D
                    {
                        string op1 = rhsTokens[0].SemanticValue;
                        string op = rhsTokens[1].SemanticValue;
                        string op2 = rhsTokens[2].SemanticValue;
                        resultingSemanticValue = NewMatrixTemp();
                        logicalMatrix.Add(new Quartet(op, op1, op2, resultingSemanticValue));
                    }
                    else if (delete == 1 && rhsTokens[0].Type == "N" && rhsTokens[0].Number == (int)N.D) // C -> D
                    {
                        resultingSemanticValue = rhsTokens[0].SemanticValue; // Передать семантическое значение D.
                    }
                    break;

                case N.B: // B -> B && C | C
                    if (delete == 3 && rhsTokens[1].Value == "&&") // B -> B && C
                    {
                        string op1_B_prev = rhsTokens[0].SemanticValue; // Результат предыдущего B
                        string op2_C = rhsTokens[2].SemanticValue;      // Результат C
                        resultingSemanticValue = NewMatrixTemp();
                        logicalMatrix.Add(new Quartet("&&", op1_B_prev, op2_C, resultingSemanticValue));
                    }
                    else if (delete == 1 && rhsTokens[0].Type == "N" && rhsTokens[0].Number == (int)N.C) // B -> C
                    {
                        resultingSemanticValue = rhsTokens[0].SemanticValue; // Передать семантическое значение C
                    }
                    break;

                case N.A: // A -> A || B | B 
                    if (delete == 3 && rhsTokens[1].Value == "||") // A -> A || B
                    {
                        string op1_A_prev = rhsTokens[0].SemanticValue; // Результат предыдущего A
                        string op2_B = rhsTokens[2].SemanticValue;      // Результат B
                        resultingSemanticValue = NewMatrixTemp();
                        logicalMatrix.Add(new Quartet("||", op1_A_prev, op2_B, resultingSemanticValue));
                    }
                    else if (delete == 1 && rhsTokens[0].Type == "N" && rhsTokens[0].Number == (int)N.B) // A -> B
                    {
                        resultingSemanticValue = rhsTokens[0].SemanticValue; // Передать семантическое значение B
                    }
                    break;
                case N.oper: //  В правилах, где переменной присваивается значение или она инкрементируется/декрементируется, нужно установить masInitialized в true
                    // oper -> ID = vir ; 
                    if (delete == 4 && rhsTokens[0].Type == "I" && rhsTokens[1].Value == "=")
                    {
                        string varName = rhsTokens[0].Value;
                        int varIndex = maS.IndexOf(varName);
                        string sourceValue = rhsTokens[2].SemanticValue; // Результат выражения <vir>
                        GenerateCode($"ПРИСВОИТЬ_ЗНАЧЕНИЕ {varName} = {sourceValue}");
                        if (varIndex != -1 && varIndex < masInitialized.Length && masType[varIndex] != -1)
                        {
                            masInitialized[varIndex] = true;
                        }
                    }
                    // oper -> тип ID = vir ;
                    else if (delete == 5 && rhsTokens[0].Type == "W" && (rhsTokens[0].Value == "int" || rhsTokens[0].Value == "long") && rhsTokens[2].Value == "=")
                    {
                        string typeName = GetRussianType(rhsTokens[0].Value);
                        string varName = rhsTokens[1].Value; // ID это rhsTokens[1]
                        int varIndex = maS.IndexOf(varName);
                        string sourceValue = rhsTokens[3].SemanticValue; // Результат выражения <vir>
                        GenerateCode($"ОБЪЯВИТЬ_ПЕРЕМЕННУЮ {varName} ТИПА {typeName}");
                        GenerateCode($"ПРИСВОИТЬ_ЗНАЧЕНИЕ {varName} = {sourceValue}");
                        if (varIndex != -1 && varIndex < masInitialized.Length && masType[varIndex] != -1)
                        {
                            masInitialized[varIndex] = true;
                        }
                    }
                    // oper -> ++ ID ;
                    else if (delete == 3 && rhsTokens[0].Value == "++" && rhsTokens[1].Type == "I")
                    {
                        GenerateCode($"УВЕЛИЧИТЬ_НА_ЕДИНИЦУ {rhsTokens[1].Value}");
                        string varName = rhsTokens[1].Value;
                        int varIndex = maS.IndexOf(varName);
                        if (varIndex != -1 && varIndex < masInitialized.Length && masType[varIndex] != -1)
                        {
                            masInitialized[varIndex] = true;
                        }
                    }
                    // oper -> ID ++ ;
                    else if (delete == 3 && rhsTokens[0].Type == "I" && rhsTokens[1].Value == "++")
                    {
                        GenerateCode($"УВЕЛИЧИТЬ_НА_ЕДИНИЦУ {rhsTokens[0].Value}");
                        string varName = rhsTokens[0].Value;
                        int varIndex = maS.IndexOf(varName);
                        if (varIndex != -1 && varIndex < masInitialized.Length && masType[varIndex] != -1)
                        {
                            masInitialized[varIndex] = true;
                        }

                    }
                    // oper -> тип ID ;  (например, int a;)

                    else if (delete == 3 && rhsTokens[0].Type == "W" && rhsTokens[2].Value == ";")
                    {
                        string typeName = GetRussianType(rhsTokens[0].Value);
                        string varName = rhsTokens[1].Value;
                        GenerateCode($"ОБЪЯВИТЬ_ПЕРЕМЕННУЮ {varName} ТИПА {typeName}");
                    }
                    break;
   
                case N.prog: // prog -> main ( ) { spis_op }
                    if (delete == 6) // main, (, ), {, spis_op, }
                    {
                      
                        GenerateCode($"КОНЕЦ_ПРОГРАММЫ");
                    }
                    break;
                // Арифметическое выражение
                case N.vir: // <выр> ::= <эл> | <эл><оп><эл>
                    if (delete == 1) // vir -> el
                    {
                        if (rhsTokens[0] != null)
                        { // Добавлена проверка
                            resultingSemanticValue = rhsTokens[0].SemanticValue;
                        }

                    }
                    else if (delete == 3) // vir -> el op el
                    {
                        string op1 = rhsTokens[0].SemanticValue;
                        string opSymbol = rhsTokens[1].SemanticValue;
                        string op2 = rhsTokens[2].SemanticValue;

                        resultingSemanticValue = NewArithmeticTemp(); // T_ARITH1, T_ARITH2, ...

                        // Проверка и генерация неявного приведения типов
                        GenerateImplicitTypeConversion(op1, op2, resultingSemanticValue);
                       
                        string command= "";
                        switch (opSymbol)
                        {
                            case "+": command = "СЛОЖИТЬ"; GenerateCode($"{command} {op1} И {op2}, РЕЗУЛЬТАТ В {resultingSemanticValue}"); break;
                            case "-": command = "ВЫЧЕСТЬ"; GenerateCode($"{command} {op2} ИЗ {op1}, РЕЗУЛЬТАТ В {resultingSemanticValue}"); break;
                            case "*": command = "УМНОЖИТЬ"; GenerateCode($"{command} {op1} НА {op2}, РЕЗУЛЬТАТ В {resultingSemanticValue}"); break;
                            case "/": command = "РАЗДЕЛИТЬ"; GenerateCode($"{command} {op1} НА {op2}, РЕЗУЛЬТАТ В {resultingSemanticValue}"); break;
                            case "%": command = "ОСТАТОК_ОТ_ДЕЛЕНИЯ"; GenerateCode($"{command} {op1} НА {op2}, РЕЗУЛЬТАТ В {resultingSemanticValue}"); break;
                        }
           
                    }
                    break;
                case N.op: 
                           // Например, когда SinALR вызывает Privedenie(1, N.op) после сдвига '+'
                    if (delete == 1 && rhsTokens[0] != null && rhsTokens[0].Type == "R") // "R" - это тип оператора (разделитель)
                    {
                        resultingSemanticValue = rhsTokens[0].Value; // Сохраняем сам символ операции
                    }
                   
                    break;


            }

            StackSost.Pop(delete);   // Извлечь 'delete' состояний из стека состояний
            StackRazbor.Pop(delete); // Извлечь 'delete' символов из стека разбора

            Token newNonTerminal = new Token("N", name.ToString(), (int)name); // Создать новый токен-нетерминал
            newNonTerminal.SemanticValue = resultingSemanticValue; // Присвоить ему вычисленное семантическое значение
            StackRazbor.Push(newNonTerminal); // Поместить новый нетерминал в стек разбора
                                              // Затем цикл SinALR выполнит переход GOTO на основе этого нового нетерминала
        }

        private void ErrorSin(int kod, int index, int sost)
        {
            string errorMessage = $"Синтаксическая ошибка #{kod} на позиции {index} в состоянии {sost}: ";
            switch (kod)
            {
                case -1:
                    errorMessage = "Синтаксических ошибок нет";
                    break;
                case 0:
                    errorMessage += "Ожидалось ключевое слово 'main'";
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
                    errorMessage += "Ожидалcя 'ID'";
                    break;
                case 16:
                    errorMessage += "Ожидались '=', '++'";
                    break;
                case 17:
                    errorMessage += "Ожидались '=', ';'";
                    break;
                case 18:
                    errorMessage += "Ожидались 'ID', 'L'";
                    break;
                case 19:
                    errorMessage += "Ожидались ';' или арифметический оператор";
                    break;
                case 20:
                    errorMessage += "Ожидались '!', '(', 'ID', 'L'";
                    break;
                case 21:
                    errorMessage += "Ожидались ')', '||'";
                    break;
                case 22:
                    errorMessage += "Ожидались ';' или '{' ,'int','long','ID','while','++'";
                    break;
                case 23:
                    errorMessage += "Ожидались '}' ,'int','long','ID','while','++'";
                    break;
                case 24:
                    errorMessage += "Ожидались '||', '&&'";
                    break;
                case 25:
                    errorMessage += "Ожидались ')', '&&', '||', 'оп_ср'";
                    break;
                case 26:
                    errorMessage += "Ожидались ')', '&&', '||'";
                    break;
                case 27:
                    errorMessage += "Ожидались 'ID', 'L', '('";
                    break;
                case 28:
                    errorMessage += "Ожидались 'int','long','ID','while','++'";
                    break;
              
                   
                case 101:
                    errorMessage += "Ошибка приведения"; // Обычно опустошение стека или несоответствие при свертке
                    break;
                default:
                    errorMessage += "Неизвестная синтаксическая ошибка";
                    break;
            }

            int currentTokenIndex = index;
            if (currentTokenIndex < 0) currentTokenIndex = 0;


            if (currentTokenIndex < Tokens.Count)
            {
                errorMessage += $". Обнаружено: {GetRussianTokenType(Tokens[currentTokenIndex].Type)}";
                if (!string.IsNullOrEmpty(Tokens[currentTokenIndex].Value))
                {
                    errorMessage += $" '{Tokens[currentTokenIndex].Value}'";
                }
            }
            else
            {
                errorMessage += ". Достигнут конец входных данных";
            }

            if (listBox != null)
            {
                listBox.Items.Add(errorMessage);
            }
            else
            {
                Console.WriteLine(errorMessage);
            }
        }

        private bool hasSemanticErrors = false; // Флаг для отслеживания возникновения семантических ошибок

        private void ErrorSem(int kod, int index, string varName = "")
        {
            if (kod != -1) // Любой код, кроме -1, является фактической ошибкой
            {
                hasSemanticErrors = true;
            }
            string errorMessage = $"Семантическая ошибка #{kod} на позиции {index}: ";

            switch (kod)
            {
                case 40: errorMessage += $"Повторное объявление переменной '{varName}'"; break;
                case 41: errorMessage += $"Использование необъявленной переменной '{varName}'"; break;
                case 42: errorMessage += $"Использование переменной '{varName}', которой не присвоено значение"; break;
                case -1: errorMessage = "Семантических ошибок нет."; break;
                default: errorMessage += "Неизвестная семантическая ошибка."; break;
            }

            if (listBox2 != null)
            {
                listBox2.Items.Add(errorMessage);
            }
            else
            {
                Console.WriteLine(errorMessage);
            }
        }
        // Семантическое действие: проверяет, объявлен ли идентификатор (для L-value (переменных в левой части присваивания))
        private void CheckDeclared(Token idToken)
        {
            string varName = idToken.Value;
            int varIndex = maS.IndexOf(varName);

            if (varIndex == -1)
            {
                // Идентификатор отсутствует в списке maS, что не должно происходить,
                // если лексер корректно добавил все идентификаторы.
                ErrorSem(41, index, varName); // Ошибка 41: Использование необъявленной переменной
                return;
            }

            // Проверка выхода за границы массива (на случай >100 уникальных идентификаторов)
            if (varIndex >= masType.Length)
            {
                ErrorSem(41, index, varName); // Рассматриваем как необъявленную, если вне области отслеживания
                return;
            }

            if (masType[varIndex] == -1) // Тип не присвоен = не объявлена
            {
                ErrorSem(41, index, varName); // Ошибка 41: Использование необъявленной переменной
            }
        }
        // Семантическое действие: вызывается при объявлении идентификатора
        private void DeclaringID(Token typeToken, Token idToken)
        {
            string varName = idToken.Value;
            int varType = typeToken.Number == 1 ? 1 : 2; // 1 для 'int' (W,1), 2 для 'long' (W,2)

            int varIndex = maS.IndexOf(varName);

            if (varIndex == -1)
            {
                // Эта ситуация маловероятна, если Lexan корректно добавляет все идентификаторы в Lexan.Identifiers (maS)
                // Если все же возникла, считаем это использованием необъявленного ID.
                ErrorSem(41, index, varName);
                return;
            }

          

            if (masType[varIndex] != -1) // Если тип уже присвоен
            {
                ErrorSem(40, index, varName); // Ошибка 40: Повторное объявление
            }
            else
            {
                // Присваиваем тип. masInitialized[varIndex] уже false по умолчанию.
                masType[varIndex] = varType;
            }
        }

        // Семантическое действие: вызывается при использовании идентификатора (как R-value)
        // Проверяет, была ли переменная объявлена ранее и инициализирована
        private void UsingID(Token idToken)
        {
            string varName = idToken.Value;
            int varIndex = maS.IndexOf(varName);

            if (varIndex == -1)
            {
                // Идентификатор отсутствует в списке maS
                ErrorSem(41, index, varName); // Ошибка 41: Использование необъявленной переменной
                return;
            }

            if (masType[varIndex] == -1) // Тип не присвоен = не объявлена
            {
                ErrorSem(41, index, varName); // Ошибка 41: Использование необъявленной переменной
            }
            else if (!masInitialized[varIndex]) // Объявлена, но не инициализирована
            {
                ErrorSem(42, index, varName); // Ошибка 42: Использование неинициализированной переменной
            }
            // Если объявлена и инициализирована, ошибок нет
        }


        // Вспомогательный метод для получения русских названий типов токенов для сообщений об ошибках
        private string GetRussianTokenType(string type)
        {
            switch (type)
            {
                case "W": return "служебное слово";
                case "R": return "оператор";
                case "I": return "идентификатор";
                case "L": return "литерал";
                case "N": return "нетерминал";
                case "Z": return "конец файла";       // '$' или маркер конца файла
                default: return type;
            }
        }
        // Перечисление для нетерминальных символов грамматики.
        public enum N { prog, spis_op, oper, op_c_while, vir, el, op, A, B, C, D, G, block, op_sr }

        public int SinALR()
        {


            while (true)
            {
                if (StackSost.isEmpty())
                {
                    ErrorSin(101, index, -1); 
                    break;
                }
                int currentState = StackSost.Read(); // Получить текущее состояние с вершины стека состояний
                Token lookaheadToken;                // Следующий входной токен.
                Token stackTopToken = StackRazbor.isEmpty() ? null : StackRazbor.Read(); // Символ на вершине стека разбора (для GOTO)


                if (index < Tokens.Count)
                {
                    lookaheadToken = Tokens[index]; // Получить следующий токен из входного потока.
                }
                else
                {
                    lookaheadToken = new Token("Z", "$", -1); // Маркер конца ввода.
                }


                switch (currentState)
                {
                    case 0: // Начальное состояние
                        if (stackTopToken != null && stackTopToken.Type == "N" && stackTopToken.Number == (int)N.prog)
                        { // GOTO по 'prog'
                            if (lookaheadToken.Type == "Z")
                            { // Z - это '$' (конец ввода)
                                ErrorSin(-1, index, currentState); // -1: Нет синтаксических ошибок
                                if (!hasSemanticErrors) ErrorSem(-1, index); // Проверить семантические ошибки, если синтаксис в порядке
                                return 0;
                            }
                        }
                        if (lookaheadToken.Type == "W" && lookaheadToken.Number == 0) { Sdvig(); Perehod(1); continue; } // Сдвиг 'main'
                        ErrorSin(0, index, currentState); break; // Ожидалось 'main'

                    case 1: // После 'main'
                        if (lookaheadToken.Type == "R" && lookaheadToken.Number == 0) { Sdvig(); Perehod(2); continue; } // Сдвиг '('
                        ErrorSin(10, index, currentState); break; // Ожидалась '('

                    case 2: // После 'main ('
                        if (lookaheadToken.Type == "R" && lookaheadToken.Number == 1) { Sdvig(); Perehod(3); continue; } // Сдвиг ')'
                        ErrorSin(11, index, currentState); break; // Ожидалась ')'

                    case 3: // После 'main ()'
                        if (lookaheadToken.Type == "R" && lookaheadToken.Number == 2) { GenerateCode($"НАЧАЛО_ПРОГРАММЫ"); Sdvig(); Perehod(4); continue; } // Сдвиг '{'
                        ErrorSin(12, index, currentState); break; // Ожидалась '{'


                    case 4: // Внутри блока main '{ ... }', ожидаются операторы или '}'
                            // Переходы GOTO после свертки, приводящей к нетерминалу на стеке
                   
                        if (stackTopToken != null && stackTopToken.Type == "N")
                        {
                            if (stackTopToken.Number == (int)N.spis_op) { Perehod(49); continue; } // GOTO по 'spis_op'
                            if (stackTopToken.Number == (int)N.oper) { Perehod(5); continue; }    // GOTO по 'oper'
                            if (stackTopToken.Number == (int)N.op_c_while) { Perehod(6); continue; } // GOTO по 'op_c_while'
                        }
                        // Операции СДВИГА для различных начал операторов
                        if (lookaheadToken.Type == "W" && lookaheadToken.Number == 1) { Sdvig(); Perehod(16); continue; } // Сдвиг 'int'
                        if (lookaheadToken.Type == "W" && lookaheadToken.Number == 2) { Sdvig(); Perehod(13); continue; } // Сдвиг 'long'
                        if (lookaheadToken.Type == "I") { Sdvig(); Perehod(10); continue; } // Сдвиг Идентификатора (для присваивания или ID++)
                        if (lookaheadToken.Type == "R" && lookaheadToken.Number == 16) { Sdvig(); Perehod(7); continue; } // Сдвиг '++' (для ++ID)
                        if (lookaheadToken.Type == "W" && lookaheadToken.Number == 3) { Sdvig(); Perehod(38); continue; } // Сдвиг 'while'

                        ErrorSin(28, index, currentState); break; // Ожидались 'int','long','ID','while','++'


                    case 5: // После свертки 'oper'
                        Privedenie(1, N.spis_op); // Свертка: spis_op -> oper
                        continue;

                    case 6: // После свертки 'op_c_while'
                        Privedenie(1, N.oper); // Свертка: oper -> op_c_while
                        continue;

                    // Состояния для '++ ID ;'
                    case 7: // После '++'
                        if (lookaheadToken.Type == "I") { UsingID(lookaheadToken); Sdvig(); Perehod(8); continue; } // Сдвиг ID
                        ErrorSin(15, index, currentState); break; // Ожидался ID
                    case 8: // После '++ ID'
                        if (lookaheadToken.Type == "R" && lookaheadToken.Number == 4) { Sdvig(); Perehod(9); continue; } // Сдвиг ';'
                        ErrorSin(14, index, currentState); break; // Ожидалась ';'
                    case 9: // После '++ ID ;'
                        Privedenie(3, N.oper); // Свертка: oper -> ++ ID ;
                        continue;

                    // Состояния для 'ID ...' (может быть присваивание или ID++)
                    case 10: // После 'ID' (может быть начало 'ID = vir;' или 'ID++;')
                        Token idToken_state10 = StackRazbor.Read(); // ID, который только что был помещен на стек

                        if (lookaheadToken.Type == "R" && lookaheadToken.Number == 16) // Следующий '++'
                        {
                            UsingID(idToken_state10); // ID используется как R-value (и L-value)
                        }
                        else if (lookaheadToken.Type == "R" && lookaheadToken.Number == 12) // Следующий '='
                        {
                            CheckDeclared(idToken_state10); // ID используется как L-value, проверяем только объявление
                        }
                        // Иначе будет синтаксическая ошибка ниже

                        if (lookaheadToken.Type == "R" && lookaheadToken.Number == 12) { Sdvig(); Perehod(19); continue; } // Сдвиг '=' (для присваивания)
                        if (lookaheadToken.Type == "R" && lookaheadToken.Number == 16) { Sdvig(); Perehod(11); continue; } // Сдвиг '++' (для ID++)
                        ErrorSin(16, index, currentState); break; // Ожидался '=' или '++'

                    // Состояния для 'ID ++ ;'
                    case 11: // После 'ID ++'
                        if (lookaheadToken.Type == "R" && lookaheadToken.Number == 4) { Sdvig(); Perehod(12); continue; } // Сдвиг ';'
                        ErrorSin(14, index, currentState); break; // Ожидалась ';'
                    case 12: // После 'ID ++ ;'
                        Privedenie(3, N.oper); // Свертка: oper -> ID ++ ;
                        continue;

                    // Состояния для 'long ID ... ;'
                    case 13: // После 'long'
                        if (lookaheadToken.Type == "I")
                        {
                            Token typeToken_13 = StackRazbor.Read(); // токен 'long'
                            DeclaringID(typeToken_13, lookaheadToken); // Объявить ID
                            Sdvig(); Perehod(14); continue; // Сдвиг ID
                        }
                        ErrorSin(15, index, currentState); break; // Ожидался ID
                    case 14: // После 'long ID'
                        if (lookaheadToken.Type == "R" && lookaheadToken.Number == 12) { Sdvig(); Perehod(35); continue; } // Сдвиг '=' (long ID = vir ;)
                        if (lookaheadToken.Type == "R" && lookaheadToken.Number == 4) { Sdvig(); Perehod(15); continue; }  // Сдвиг ';' (long ID ;)
                        ErrorSin(17, index, currentState); break; // Ожидался '=' или ';'
                    case 15: // После 'long ID ;'
                        Privedenie(3, N.oper); // Свертка: oper -> long ID ;
                        continue;

                    // Состояния для 'int ID ... ;'
                    case 16: // После 'int'
                        if (lookaheadToken.Type == "I")
                        {
                            Token typeToken_16 = StackRazbor.Read(); // токен 'int'
                            DeclaringID(typeToken_16, lookaheadToken); // Объявить ID
                            Sdvig(); Perehod(17); continue; // Сдвиг ID
                        }
                        ErrorSin(15, index, currentState); break; // Ожидался ID
                    case 17: // После 'int ID'
                        if (lookaheadToken.Type == "R" && lookaheadToken.Number == 12) { Sdvig(); Perehod(32); continue; } // Сдвиг '=' (int ID = vir ;)
                        if (lookaheadToken.Type == "R" && lookaheadToken.Number == 4) { Sdvig(); Perehod(18); continue; }  // Сдвиг ';' (int ID ;)
                        ErrorSin(17, index, currentState); break; // Ожидался '=' или ';'
                    case 18: // После 'int ID ;'
                        Privedenie(3, N.oper); // Свертка: oper -> int ID ;
                        continue;

                    // Состояния для выражения присваивания 'ID = vir' (vir - арифметическое выражение)
                    case 19: // После 'ID ='
                        // Переходы GOTO для частей 'vir'
                        if (stackTopToken != null && stackTopToken.Type == "N")
                        {
                            if (stackTopToken.Number == (int)N.vir) { Perehod(30); continue; } // GOTO по 'vir'
                            if (stackTopToken.Number == (int)N.el) { Perehod(22); continue; }  // GOTO по 'el'
                        }
                        // СДВИГ для 'el' (ID или Литерал)
                        if (lookaheadToken.Type == "I") { UsingID(lookaheadToken); Sdvig(); Perehod(20); continue; } // Сдвиг ID
                        if (lookaheadToken.Type == "L") { Sdvig(); Perehod(21); continue; } // Сдвиг Литерала
                        ErrorSin(18, index, currentState); break; // Ожидался ID или Литерал

                    case 20: // После 'ID' в выражении 
                        // UsingID(StackRazbor.Read()); // Уже проверено при сдвиге в 20 из 19, 28, 32, 35, 39, 51, 53, 61, 65, 68
                        Privedenie(1, N.el); // Свертка: el -> I
                        continue;
                    case 21: // После 'L' (Литерал) в выражении
                        Privedenie(1, N.el); // Свертка: el -> L
                        continue;
                    case 22:
                        if (stackTopToken != null && stackTopToken.Type == "N" && stackTopToken.Number == (int)N.el &&
                            lookaheadToken.Type == "R" && lookaheadToken.Number == 4)
                        { // el ;
                            Privedenie(1, N.vir); // Свертка: vir -> el
                            continue;
                        }
                        // GOTO по 'op', если арифметический оператор был свернут
                        if (stackTopToken != null && stackTopToken.Type == "N" && stackTopToken.Number == (int)N.op)
                        {
                            Perehod(28); // GOTO в состояние 28 (после el op)
                            continue;
                        }
                        // СДВИГ арифметических операторов
                        if (lookaheadToken.Type == "R")
                        {
                            if (lookaheadToken.Number == 9) { Sdvig(); Perehod(23); continue; } // Сдвиг '+'
                            if (lookaheadToken.Number == 8) { Sdvig(); Perehod(24); continue; } // Сдвиг '-'
                            if (lookaheadToken.Number == 5) { Sdvig(); Perehod(25); continue; } // Сдвиг '*'
                            if (lookaheadToken.Number == 6) { Sdvig(); Perehod(26); continue; } // Сдвиг '/'
                            if (lookaheadToken.Number == 7) { Sdvig(); Perehod(27); continue; } // Сдвиг '%'
                        }
                        ErrorSin(19, index, currentState); // Ожидалась ';' или арифметический оператор
                        break;

                    // Свертка арифметических операторов в нетерминал 'op'
                    case 23: Privedenie(1, N.op); continue; // op -> +
                    case 24: Privedenie(1, N.op); continue; // op -> -
                    case 25: Privedenie(1, N.op); continue; // op -> *
                    case 26: Privedenie(1, N.op); continue; // op -> /
                    case 27: Privedenie(1, N.op); continue; // op -> %   

                    case 28: // После 'el op', ожидается еще один 'el' для 'vir -> el op el'
                        // GOTO по 'el'
                        if (stackTopToken != null && stackTopToken.Type == "N" && stackTopToken.Number == (int)N.el)
                        {
                            Perehod(29); continue; // GOTO в состояние 29 (после el op el)
                        }
                        // СДВИГ для 'el' (ID или Литерал)
                        if (lookaheadToken.Type == "I") { UsingID(lookaheadToken); Sdvig(); Perehod(20); continue; } // Сдвиг ID
                        if (lookaheadToken.Type == "L") { Sdvig(); Perehod(21); continue; } // Сдвиг Литерала
                        ErrorSin(18, index, currentState); break; // Ожидался ID или Литерал

                    case 29: // После 'el op el'
                        Privedenie(3, N.vir); // Свертка: vir -> el op el
                        continue;

                    case 30: // После 'vir' в 'ID = vir', ожидается ';'
                        if (lookaheadToken.Type == "R" && lookaheadToken.Number == 4) { Sdvig(); Perehod(31); continue; } // Сдвиг ';'
                        ErrorSin(14, index, currentState); break; // Ожидалась ';'
                    case 31: // После 'ID = vir ;'
                        Privedenie(4, N.oper); // Свертка: oper -> ID = vir ; (ID, =, vir, ;)
                        continue;

                    // Состояния для 'int ID = vir ;'
                    case 32: // После 'int ID ='
                        if (stackTopToken != null && stackTopToken.Type == "N")
                        {
                            if (stackTopToken.Number == (int)N.vir) { Perehod(33); continue; } // GOTO по 'vir'
                            if (stackTopToken.Number == (int)N.el) { Perehod(22); continue; }  // GOTO по 'el'
                        }
                        if (lookaheadToken.Type == "I") { UsingID(lookaheadToken); Sdvig(); Perehod(20); continue; }
                        if (lookaheadToken.Type == "L") { Sdvig(); Perehod(21); continue; }
                        ErrorSin(18, index, currentState); break; // Ожидались 'ID', 'L'"

                    case 33: // После 'int ID = vir'
                        if (lookaheadToken.Type == "R" && lookaheadToken.Number == 4) { Sdvig(); Perehod(34); continue; } // Сдвиг ';'
                        ErrorSin(14, index, currentState); break; // Ожидалась точка с запятой ';'

                    case 34: // После 'int ID = vir ;'
                        Privedenie(5, N.oper); // Свертка: oper -> int ID = vir ; (int, ID, =, vir, ;)
                        continue;

                    // Состояния для 'long ID = vir ;'
                    case 35: // После 'long ID ='
                        if (stackTopToken != null && stackTopToken.Type == "N")
                        {
                            if (stackTopToken.Number == (int)N.vir) { Perehod(36); continue; } // GOTO по 'vir'
                            if (stackTopToken.Number == (int)N.el) { Perehod(22); continue; }  // GOTO по 'el'
                        }
                        if (lookaheadToken.Type == "I") { UsingID(lookaheadToken); Sdvig(); Perehod(20); continue; }
                        if (lookaheadToken.Type == "L") { Sdvig(); Perehod(21); continue; }
                        ErrorSin(18, index, currentState); break; // Ожидались 'ID', 'L'"
                    case 36: // После 'long ID = vir'
                        if (lookaheadToken.Type == "R" && lookaheadToken.Number == 4) { Sdvig(); Perehod(37); continue; } // Сдвиг ';'
                        ErrorSin(14, index, currentState); break; // Ожидалась точка с запятой ';'
                    case 37: // После 'long ID = vir ;'
                        Privedenie(5, N.oper); // Свертка: oper -> long ID = vir ; (long, ID, =, vir, ;)
                        continue;

                    // Состояния для 'while (A) block' (op_c_while)
                    case 38: // После 'while'
                        if (lookaheadToken.Type == "R" && lookaheadToken.Number == 0)
                        {  // Генерируем метки для цикла while и помещаем их в стек
                            string labelPrefix = GetNextLabelPrefix("МЕТКА_ЦИКЛА_WHILE");
                            string conditionLabel = labelPrefix + "_НАЧАЛО";
                            string endLabel = labelPrefix + "_КОНЕЦ";

                            loopLabelStack.Push(conditionLabel);
                            //loopLabelStack.Push(bodyLabel);
                            loopLabelStack.Push(endLabel);

                            GenerateCode($"{conditionLabel}:"); // Метка начала проверки условия
                             Sdvig(); Perehod(39); continue; } // Сдвиг '('
                            ErrorSin(10, index, currentState); break; // Ожидалась '('

                    case 39: // После 'while (', ожидается логическое выражение 'A'
                        // Переходы GOTO для компонентов логического выражения A
                        if (stackTopToken != null && stackTopToken.Type == "N")
                        {
                            if (stackTopToken.Number == (int)N.A) { Perehod(42); continue; } // GOTO по A (полное логическое выражение)
                            if (stackTopToken.Number == (int)N.B) { Perehod(63); continue; } // GOTO по B
                            if (stackTopToken.Number == (int)N.C) { Perehod(64); continue; } // GOTO по C
                            if (stackTopToken.Number == (int)N.D) { Perehod(41); continue; } // GOTO по D
                            if (stackTopToken.Number == (int)N.G) { Perehod(40); continue; } // GOTO по G
                            if (stackTopToken.Number == (int)N.el) { Perehod(70); continue; } // GOTO по el 
                        }
                        // СДВИГ для первых элементов логического выражения
                        if (lookaheadToken.Type == "R" && lookaheadToken.Number == 13) { Sdvig(); Perehod(68); continue; } // Сдвиг '!' (для !G)
                        if (lookaheadToken.Type == "R" && lookaheadToken.Number == 0) { Sdvig(); Perehod(65); continue; }  // Сдвиг '(' (для (A))
                        if (lookaheadToken.Type == "I") { UsingID(lookaheadToken); Sdvig(); Perehod(20); continue; } // Сдвиг ID (для el)
                        if (lookaheadToken.Type == "L") { Sdvig(); Perehod(21); continue; } // Сдвиг Литерала (для el)
                        ErrorSin(20, index, currentState); break; // Ожидались '!', '(', ID или L

                    case 40: Privedenie(1, N.D); continue; // D -> G
                    case 41: Privedenie(1, N.C); continue; // C -> D

                    case 42: // После 'A' в 'while (A ...' (A - полное логическое выражение)
                        if (lookaheadToken.Type == "R" && lookaheadToken.Number == 1)
                        {    
                            Token conditionTokenA = StackRazbor.Read();
                            string conditionResult = conditionTokenA.SemanticValue;

                            // Преобразование целочисленного условия в логическое, если необходимо
                            conditionResult = EnsureBooleanCondition(conditionResult);
                            // Обновляем SemanticValue токена A, если он был преобразован
                            conditionTokenA.SemanticValue = conditionResult;

                            // Извлекаем метки из стека (без удаления, peek)
                            string currentEndLabel = loopLabelStack.mas[loopLabelStack.top];     // Конец
                     

                            GenerateCode($"    ПРОВЕРКА_УСЛОВИЯ_ЦИКЛА_WHILE: ({conditionResult})");
                            GenerateCode($"    ЕСЛИ_УСЛОВИЕ_ЛОЖНО_ПЕРЕЙТИ_НА_МЕТКУ {currentEndLabel}"); 
                            Sdvig(); Perehod(43); continue; }  // Сдвиг ')'
                        if (lookaheadToken.Type == "R" && lookaheadToken.Number == 21) { Sdvig(); Perehod(51); continue; } // Сдвиг '||' (для A -> A || B)
                        ErrorSin(21, index, currentState); break; // Ожидались ')' или '||'

                    case 43: // После 'while (A)', ожидается 'block' (либо один 'oper', либо '{ spis_op }')
                        // GOTO для 'block' или его компонентов
                        if (stackTopToken != null && stackTopToken.Type == "N")
                        {
                            if (stackTopToken.Number == (int)N.block) { Perehod(48); continue; } // GOTO по 'block'
                            if (stackTopToken.Number == (int)N.oper) { Perehod(5); continue; }   // GOTO по 'oper'
                            if (stackTopToken.Number == (int)N.op_c_while) { Perehod(6); continue; } // GOTO по 'op_c_while' (также является 'oper')
                        }
                        // СДВИГ для начал 'block'
                        if (lookaheadToken.Type == "R" && lookaheadToken.Number == 2) { Sdvig(); Perehod(45); continue; }   // Сдвиг '{' (для block -> { spis_op })
                        // Ниже - сдвиги для 'oper', если block - это один оператор
                        if (lookaheadToken.Type == "W" && lookaheadToken.Number == 1) { Sdvig(); Perehod(16); continue; }  // Сдвиг 'int'
                        if (lookaheadToken.Type == "W" && lookaheadToken.Number == 2) { Sdvig(); Perehod(13); continue; }  // Сдвиг 'long'
                        if (lookaheadToken.Type == "I") { Sdvig(); Perehod(10); continue; } // Сдвиг ID
                        if (lookaheadToken.Type == "R" && lookaheadToken.Number == 16) { Sdvig(); Perehod(7); continue; }  // Сдвиг '++'
                        if (lookaheadToken.Type == "W" && lookaheadToken.Number == 3) { Sdvig(); Perehod(38); continue; }  // Сдвиг 'while' (вложенный while)
                        if (lookaheadToken.Type == "R" && lookaheadToken.Number == 4) { Sdvig(); Perehod(44); continue; } // Сдвиг ';'
                        ErrorSin(22, index, currentState); break; // Ожидались ';', '{' ,'int','long','ID','while','++'

                    case 44: Privedenie(1, N.block); continue; // block -> ; (пустой блок)

                    case 45:  // После 'while (A) {', ожидается 'spis_op'
                        // Переходы GOTO для компонентов 'spis_op'
                        if (stackTopToken != null && stackTopToken.Type == "N")
                        {
                            if (stackTopToken.Number == (int)N.spis_op) { Perehod(46); continue; } // GOTO по 'spis_op'
                            if (stackTopToken.Number == (int)N.oper) { Perehod(5); continue; } // GOTO по 'oper' (часть spis_op)
                            if (stackTopToken.Number == (int)N.op_c_while) { Perehod(6); continue; } // GOTO по 'op_c_while' (часть oper)
                        }
                        // СДВИГ для начал операторов внутри '{...}'
                        if (lookaheadToken.Type == "W" && lookaheadToken.Number == 1) { Sdvig(); Perehod(16); continue; } // Сдвиг 'int'
                        if (lookaheadToken.Type == "W" && lookaheadToken.Number == 2) { Sdvig(); Perehod(13); continue; } // Сдвиг 'long'
                        if (lookaheadToken.Type == "I") { Sdvig(); Perehod(10); continue; }
                        if (lookaheadToken.Type == "R" && lookaheadToken.Number == 16) { Sdvig(); Perehod(7); continue; } // Сдвиг '++'
                        if (lookaheadToken.Type == "W" && lookaheadToken.Number == 3) { Sdvig(); Perehod(38); continue; } // Сдвиг 'while'


                        ErrorSin(28, index, currentState); break;  // Ожидались 'int', 'long', ID, 'while', '++'

                    case 46: // После 'while (A) { spis_op', ожидается '}' или еще 'oper' для 'spis_op -> spis_op oper'
                        // GOTO для 'oper' или 'op_c_while' (чтобы расширить spis_op)
                        if (stackTopToken != null && stackTopToken.Type == "N")
                        {
                            if (stackTopToken.Number == (int)N.oper) { Perehod(5); continue; } // spis_op -> spis_op oper (новый oper)
                            if (stackTopToken.Number == (int)N.op_c_while) { Perehod(6); continue; }
                        }
                        // СДВИГ для следующих операторов
                        if (lookaheadToken.Type == "W" && lookaheadToken.Number == 1) { Sdvig(); Perehod(16); continue; }
                        if (lookaheadToken.Type == "W" && lookaheadToken.Number == 2) { Sdvig(); Perehod(13); continue; }
                        if (lookaheadToken.Type == "I") { Sdvig(); Perehod(10); continue; }
                        if (lookaheadToken.Type == "R" && lookaheadToken.Number == 16) { Sdvig(); Perehod(7); continue; }
                        if (lookaheadToken.Type == "W" && lookaheadToken.Number == 3) { Sdvig(); Perehod(38); continue; }
                        if (lookaheadToken.Type == "R" && lookaheadToken.Number == 3) { Sdvig(); Perehod(47); continue; }  // Сдвиг '}'
                        ErrorSin(23, index, currentState); break; // Ожидались 'int', 'long', ID, 'while', '++', '}'

                    case 47: Privedenie(3, N.block); continue;  // Свертка: block -> { spis_op }      

                    case 48: // После 'block' в 'while (A) block'

                        // Извлекаем метки из стека (без удаления, peek)
                        string currentEndLabel2 = loopLabelStack.mas[loopLabelStack.top];     // Конец
                        string currentConditionLabel2 = loopLabelStack.mas[loopLabelStack.top - 1]; // Условие

                        // Генерируем безусловный переход обратно к метке проверки условия
                        GenerateCode($"    ПЕРЕЙТИ_НА_МЕТКУ {currentConditionLabel2}");

                        // Генерируем саму метку конца цикла.
                        // На эту метку будет переход из case 42, если условие ложно.
                        GenerateCode($"{currentEndLabel2}:");
                        Privedenie(5, N.op_c_while); // Свертка: op_c_while -> while ( A ) block (токены: while, (, A, ), block)
                       // GenerateCode($"КОНЕЦ_ЦИКЛА"); // Добавим явное завершение цикла
                        continue;

                    // Структура основной программы: main () { spis_op }
                    case 49: // После 'main () { spis_op', ожидается '}' или еще 'oper'
                        // GOTO (аналогично состоянию 46)
                        if (stackTopToken != null && stackTopToken.Type == "N")
                        {
                            if (stackTopToken.Number == (int)N.oper) { Perehod(5); continue; }
                            if (stackTopToken.Number == (int)N.op_c_while) { Perehod(6); continue; }
                        }
                        // СДВИГ (аналогично состоянию 46)
                        if (lookaheadToken.Type == "W" && lookaheadToken.Number == 1) { Sdvig(); Perehod(16); continue; } // Сдвиг 'int'
                        if (lookaheadToken.Type == "W" && lookaheadToken.Number == 2) { Sdvig(); Perehod(13); continue; } // Сдвиг 'long'
                        if (lookaheadToken.Type == "I") { Sdvig(); Perehod(10); continue; }
                        if (lookaheadToken.Type == "R" && lookaheadToken.Number == 16) { Sdvig(); Perehod(7); continue; } // Сдвиг '++'
                        if (lookaheadToken.Type == "W" && lookaheadToken.Number == 3) { Sdvig(); Perehod(38); continue; } // Сдвиг 'while'
                        if (lookaheadToken.Type == "R" && lookaheadToken.Number == 3) { Sdvig(); Perehod(50); continue; }  // Сдвиг '}'
                        ErrorSin(23, index, currentState); break; // "Ожидались '}' ,'int','long','ID','while','++'

                    case 50: // После 'main () { spis_op }'
                        Privedenie(6, N.prog); // Свертка: prog -> main ( ) { spis_op } (main, (, ), {, spis_op, })
                        continue;

                    // Состояния для логического выражения A -> A || B
                    case 51: // После 'A ||', ожидается 'B'
                        // GOTO для частей B
                        if (stackTopToken != null && stackTopToken.Type == "N")
                        {
                            if (stackTopToken.Number == (int)N.B) { Perehod(52); continue; } // GOTO по B
                            if (stackTopToken.Number == (int)N.C) { Perehod(64); continue; } // GOTO по C (часть B)
                            if (stackTopToken.Number == (int)N.D) { Perehod(41); continue; } // GOTO по D (часть C)
                            if (stackTopToken.Number == (int)N.G) { Perehod(40); continue; } // GOTO по G (часть D)
                            if (stackTopToken.Number == (int)N.el) { Perehod(70); continue; } // GOTO по el (часть G)
                        }
                        // СДВИГ для первых элементов B
                        if (lookaheadToken.Type == "R" && lookaheadToken.Number == 13) { Sdvig(); Perehod(68); continue; } // Сдвиг '!'
                        if (lookaheadToken.Type == "R" && lookaheadToken.Number == 0) { Sdvig(); Perehod(65); continue; }  // Сдвиг '('
                        if (lookaheadToken.Type == "I") { UsingID(lookaheadToken); Sdvig(); Perehod(20); continue; }
                        if (lookaheadToken.Type == "L") { Sdvig(); Perehod(21); continue; }
                        ErrorSin(20, index, currentState); break; // Ожидались '!', '(', ID или L


                    case 52: // После 'A || B'
                        // Если следующий токен ')', '||' (конец текущего подвыражения или следующее ||), свернуть A -> A || B
                        if (lookaheadToken.Type == "R" && (lookaheadToken.Number == 1 || lookaheadToken.Number == 21))
                        { // ')' или '||'
                            Privedenie(3, N.A); continue; // Свертка: A -> A || B
                        }
                        // Если следующий токен '&&', то B нужно расширить: B -> B && C
                        if (lookaheadToken.Type == "R" && lookaheadToken.Number == 22) { Sdvig(); Perehod(53); continue; } // Сдвиг '&&'
                        ErrorSin(26, index, currentState); break; // Ожидались ')', '||' или '&&'

                    // Состояния для логического выражения B -> B && C
                    case 53: // После 'B &&', ожидается 'C'
                        // GOTO для частей C
                        if (stackTopToken != null && stackTopToken.Type == "N")
                        {
                            if (stackTopToken.Number == (int)N.C) { Perehod(54); continue; } // GOTO по C
                            if (stackTopToken.Number == (int)N.D) { Perehod(41); continue; } // GOTO по D (часть C)
                            if (stackTopToken.Number == (int)N.G) { Perehod(40); continue; } // GOTO по G (часть D)
                            if (stackTopToken.Number == (int)N.el) { Perehod(70); continue; } // GOTO по el (часть G)
                        }
                        // СДВИГ для первых элементов C
                        if (lookaheadToken.Type == "R" && lookaheadToken.Number == 13) { Sdvig(); Perehod(68); continue; } // Сдвиг '!'
                        if (lookaheadToken.Type == "R" && lookaheadToken.Number == 0) { Sdvig(); Perehod(65); continue; }  // Сдвиг '('
                        if (lookaheadToken.Type == "I") { UsingID(lookaheadToken); Sdvig(); Perehod(20); continue; }
                        if (lookaheadToken.Type == "L") { Sdvig(); Perehod(21); continue; }
                        ErrorSin(20, index, currentState); break; // Ожидались '!', '(', ID или L

                    case 54: // После 'B && C'
                        // Если следующий токен ')', '||', '&&' (конец подвыражения или следующий оператор), свернуть B -> B && C
                        if (lookaheadToken.Type == "R" && (lookaheadToken.Number == 1 || lookaheadToken.Number == 21 || lookaheadToken.Number == 22))
                        { // ')', '||' или '&&'
                            Privedenie(3, N.B); continue; // Свертка: B -> B && C
                        }
                        // GOTO/СДВИГ для оператора сравнения, если C расширяется: C -> D op_sr D
                        if (stackTopToken != null && stackTopToken.Type == "N" && stackTopToken.Number == (int)N.op_sr)
                        { // GOTO op_sr
                            Perehod(61); continue;
                        }
                        // СДВИГ операторов сравнения (op_sr)
                        if (lookaheadToken.Type == "R" && lookaheadToken.Number == 10) { Sdvig(); Perehod(55); continue; } // Сдвиг '<'
                        if (lookaheadToken.Type == "R" && lookaheadToken.Number == 11) { Sdvig(); Perehod(56); continue; } // Сдвиг '>'
                        if (lookaheadToken.Type == "R" && lookaheadToken.Number == 18) { Sdvig(); Perehod(57); continue; } // Сдвиг '<='
                        if (lookaheadToken.Type == "R" && lookaheadToken.Number == 17) { Sdvig(); Perehod(58); continue; } // Сдвиг '>='
                        if (lookaheadToken.Type == "R" && lookaheadToken.Number == 19) { Sdvig(); Perehod(59); continue; } // Сдвиг '=='
                        if (lookaheadToken.Type == "R" && lookaheadToken.Number == 20) { Sdvig(); Perehod(60); continue; } // Сдвиг '!='
                        ErrorSin(25, index, currentState); break; // Ожидались ')', '||', '&&' или оператор сравнения

                    // Свертка операторов сравнения в нетерминал 'op_sr'
                    case 55: Privedenie(1, N.op_sr); continue; // op_sr -> <
                    case 56: Privedenie(1, N.op_sr); continue; // op_sr -> >
                    case 57: Privedenie(1, N.op_sr); continue; // op_sr -> <=
                    case 58: Privedenie(1, N.op_sr); continue; // op_sr -> >=
                    case 59: Privedenie(1, N.op_sr); continue; // op_sr -> ==
                    case 60: Privedenie(1, N.op_sr); continue; // op_sr -> !=


                    // Состояния для логического выражения C -> C op_sr D
                    case 61: // После 'D op_sr', ожидается 'D'
                        // GOTO для частей D
                        if (stackTopToken != null && stackTopToken.Type == "N")
                        {
                            if (stackTopToken.Number == (int)N.D) { Perehod(62); continue; } // GOTO по D
                            if (stackTopToken.Number == (int)N.G) { Perehod(40); continue; } // GOTO по G (часть D)
                            if (stackTopToken.Number == (int)N.el) { Perehod(70); continue; } // GOTO по el (часть G)
                        }
                        // СДВИГ для первых элементов D
                        if (lookaheadToken.Type == "R" && lookaheadToken.Number == 13) { Sdvig(); Perehod(68); continue; } // Сдвиг '!'
                        if (lookaheadToken.Type == "R" && lookaheadToken.Number == 0) { Sdvig(); Perehod(65); continue; }  // Сдвиг '('
                        if (lookaheadToken.Type == "I") { UsingID(lookaheadToken); Sdvig(); Perehod(20); continue; }
                        if (lookaheadToken.Type == "L") { Sdvig(); Perehod(21); continue; }
                        ErrorSin(20, index, currentState); break; // Ожидались '!', '(', ID или L

                    case 62: Privedenie(3, N.C); continue; // Свертка: C -> C op_sr D    

                    case 63: // После 'B' (может быть начало A, или часть A || B)
                        if (lookaheadToken.Type == "R" && (lookaheadToken.Number == 1 || lookaheadToken.Number == 21))
                        { // ')' или '||'
                            Privedenie(1, N.A); continue; // Свертка: A -> B
                        }
                        if (lookaheadToken.Type == "R" && lookaheadToken.Number == 22) { Sdvig(); Perehod(53); continue; } // Сдвиг '&&' (для B -> B && C)
                        ErrorSin(26, index, currentState); break; // Ожидались ')', '||' или '&&'

                    case 64: // После 'C' (может быть начало B, или часть B && C)
                        if (lookaheadToken.Type == "R" && (lookaheadToken.Number == 1 || lookaheadToken.Number == 21 || lookaheadToken.Number == 22))
                        { // ')', '||' или '&&'
                            Privedenie(1, N.B); continue; // Свертка: B -> C
                        }
                        // GOTO/СДВИГ для оператора сравнения, если C это C op_sr D
                        if (stackTopToken != null && stackTopToken.Type == "N" && stackTopToken.Number == (int)N.op_sr)
                        {
                            Perehod(61); continue;
                        }
                        if (lookaheadToken.Type == "R" && lookaheadToken.Number == 10) { Sdvig(); Perehod(55); continue; } // <
                        if (lookaheadToken.Type == "R" && lookaheadToken.Number == 11) { Sdvig(); Perehod(56); continue; } // >
                        if (lookaheadToken.Type == "R" && lookaheadToken.Number == 18) { Sdvig(); Perehod(57); continue; } // <=
                        if (lookaheadToken.Type == "R" && lookaheadToken.Number == 17) { Sdvig(); Perehod(58); continue; } // >=
                        if (lookaheadToken.Type == "R" && lookaheadToken.Number == 19) { Sdvig(); Perehod(59); continue; } // ==
                        if (lookaheadToken.Type == "R" && lookaheadToken.Number == 20) { Sdvig(); Perehod(60); continue; } // !=
                        ErrorSin(25, index, currentState); break; // Ожидались ')', '||', '&&' или оператор сравнения

                    case 65:   // После '(' в логическом выражении, ожидается 'A'     
                        if (stackTopToken != null && stackTopToken.Type == "N")
                        {
                            if (stackTopToken.Number == (int)N.A) { Perehod(66); continue; }
                            if (stackTopToken.Number == (int)N.B) { Perehod(63); continue; }
                            if (stackTopToken.Number == (int)N.C) { Perehod(64); continue; }
                            if (stackTopToken.Number == (int)N.D) { Perehod(41); continue; }
                            if (stackTopToken.Number == (int)N.G) { Perehod(40); continue; }
                            if (stackTopToken.Number == (int)N.el) { Perehod(70); continue; }
                        }
                        // СДВИГ для первых элементов A
                        if (lookaheadToken.Type == "R" && lookaheadToken.Number == 13) { Sdvig(); Perehod(68); continue; }    // Сдвиг '!' 
                        if (lookaheadToken.Type == "R" && lookaheadToken.Number == 0) { Sdvig(); Perehod(65); continue; }    // Сдвиг '('      
                        if (lookaheadToken.Type == "I") { UsingID(lookaheadToken); Sdvig(); Perehod(20); continue; }
                        if (lookaheadToken.Type == "L") { Sdvig(); Perehod(21); continue; }
                        ErrorSin(20, index, currentState); break; // Ожидались '!', '(', 'ID', 'L'

                    case 66: // После '( A', ожидается ')' или '||' если A -> A || B
                        if (lookaheadToken.Type == "R" && lookaheadToken.Number == 1) { Sdvig(); Perehod(67); continue; } // Сдвиг ')'
                        if (lookaheadToken.Type == "R" && lookaheadToken.Number == 21) { Sdvig(); Perehod(51); continue; } // Сдвиг '||'
                        ErrorSin(21, index, currentState); break; // Ожидались ')' или '||'

                    case 67: Privedenie(3, N.G); continue; // Свертка: G -> ( A )    

                    // Состояния для D -> ! G
                    case 68: // После '!', ожидается 'G'
                        // GOTO для частей G
                        if (stackTopToken != null && stackTopToken.Type == "N")
                        {
                            if (stackTopToken.Number == (int)N.G) { Perehod(69); continue; } // GOTO по G
                            if (stackTopToken.Number == (int)N.el) { Perehod(70); continue; } // GOTO по el (часть G)
                        }
                        // СДВИГ для первых элементов G
                        if (lookaheadToken.Type == "R" && lookaheadToken.Number == 0) { Sdvig(); Perehod(65); continue; }  // Сдвиг '(' (для G -> (A))
                        if (lookaheadToken.Type == "I") { UsingID(lookaheadToken); Sdvig(); Perehod(20); continue; } // Сдвиг ID (для G -> el -> I)
                        if (lookaheadToken.Type == "L") { Sdvig(); Perehod(21); continue; } // Сдвиг Литерала (для G -> el -> L)
                        ErrorSin(27, index, currentState); break; // Ожидались '(', ID или L

                    case 69: Privedenie(2, N.D); continue; // Свертка: D -> ! G
                    case 70: Privedenie(1, N.G); continue; // Свертка: G -> el


                    default:
                        // Обработка неизвестного состояния автомата
                        ErrorSin(1000, index, currentState);  // Генерация ошибки с кодом 1000
                        MessageBox.Show($"Неизвестное состояние {currentState} в SinALR. Аварийное завершение.");
                        return 1;  // Возврат кода ошибки 1 (критическая ошибка состояния) 
                }
                // Проверка наличия ошибок в списке ошибок (listBox)
                if (listBox.Items.Count > 0)
                {
                    // Получаем текст последней ошибки
                    string lastError = listBox.Items[listBox.Items.Count - 1].ToString();

                    // Проверяем, что это действительно ошибка (а не сообщение об отсутствии ошибок)
                    if (!lastError.Contains("Синтаксических ошибок нет") &&
                        !lastError.Contains("Семантических ошибок нет"))
                    {
                        // Игнорируем ошибку приведения #101 (она обрабатывается отдельно)
                        if (!lastError.Contains("Ошибка приведения #101"))
                        {
                            return 1;  // Возвращаем код 1 для синтаксических ошибок
                        }
                    }
                }

                // Проверка флага семантических ошибок
                if (hasSemanticErrors)
                {
                    return 2;  // Возвращаем код 2 для семантических ошибок
                }

            }
            return 0;     // Успешное завершение     
        }
    }
}