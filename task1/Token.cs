using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace task1
{
	public class Token
	{
		public string Type { get; set; }   // Тип токена
        public string Value { get; set; }   // Значение токена
        public int Number { get; set; }  //Номер токена

        public string SemanticValue { get; set; }

		public Token(string type, string value, int number = -1)
		{
			Type = type;
			Value = value;
			Number = number;
			if (type == "I" || type == "L" || (type == "R" && (value == "!" || value == "&&" || value == "" || IsComparisonOperator(value))))
			{
				SemanticValue = value;
			}
			else
			{
				SemanticValue = null;
			}
		}
		private static bool IsComparisonOperator(string val)
		{
			return val == "<" || val == ">" || val == "<=" || val == ">=" || val == "==" || val == "!=";
		}



		public override string ToString()
		{
			return $"({Type}, {Number})";
		}
	}

	public class Lexan
	{
		public Lexan()
		{ }
		enum State { S, I, H, R, D }       // Состояния: S - начальное, I - идентификатор, H - шестнадцатеричное число, R - разделитель, D - десятичное число

        // Проверка, является ли символ английской буквой             

        private static bool IsEnglishLetter(char c)
		{
			return (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z');
		}
        // Проверка, является ли символ шестнадцатеричной цифрой
        private static bool IsHexDigit(char c)
		{
			return (c >= '0' && c <= '9') || (c >= 'a' && c <= 'f') || (c >= 'A' && c <= 'F');
		}
        // Проверка, является ли символ десятичной цифрой
        private static bool IsDecimalDigit(char c)
		{
			return (c >= '0' && c <= '9');
		}
        // Проверка, является ли символ пробелом
        private static bool IsWhiteSpace(char c)
		{
			return c == ' ' || c == '\t' || c == '\n' || c == '\r';
		}
        // Проверка, является ли символ буквой или цифрой
        private static bool IsLetterOrDigit(char c)
		{
			return IsEnglishLetter(c) || IsHexDigit(c);
		}
        // Проверка, является ли начало строки шестнадцатеричным числом
        private static bool IsHexDigitStart(string inputString, int index)
		{
			return inputString[index] == '0' && index + 1 < inputString.Length && (inputString[index + 1] == 'x' || inputString[index + 1] == 'X');
		}
        // Таблицы лексем                             0       1      2       3
        private static readonly string[] SlWord = { "main", "int", "long", "while" };
        //                                                 0    1    2    3    4    5    6    7     8
        private static readonly string[] SingleRazdel = { "(", ")", "{", "}", ";", "*", "/", "%", "-" };
        //                                               9    10   11   12   13   14   15
        private static readonly string[] ParnRazdel = { "+", ">", "<", "=", "!", "&", "|" };
        //                                               16   17    18    19    20    21    22
        private static readonly string[] TwoRazdel = { "++", "<=", ">=", "==", "!=", "||", "&&" };
        // Таблицы для хранения идентификаторов и литералов
        public static readonly List<string> Identifiers = new List<string>();
		public static readonly List<string> Literals = new List<string>();

		public void Error(int kod, ListBox listBox3, char? unknownChar = null) 
		{
			string errorMessage = $"Лексическая ошибка #{kod}: ";

			switch (kod)
			{
				case -1:
					errorMessage = "Лексических ошибок нет.";
					break;
				case 101:
					errorMessage += $"Неизвестный символ '{(unknownChar.HasValue ? unknownChar.Value.ToString() : " ")}'";
					break;
                case 102:
                    errorMessage += "Шестнадцатеричное число должно быть 2-байтным (0x0000-0xFFFF) или 4-байтным (0x00000000-0xFFFFFFFF)";
                    break;
                case 103:
                    errorMessage += "Десятичное число должно быть 2-байтным (0-65535) или 4-байтным (0-4294967295)";
                    break;
                default:
					errorMessage += "Неизвестная лексическая  ошибка";
					break;
			}
			if (listBox3 != null)
			{
				listBox3.Items.Add(errorMessage);
			}
			else
			{
				Console.WriteLine(errorMessage);
			}
		}
		public static List<Token> InterpString(string inputString, ListBox listBox3)
		{
			List<Token> Tokens = new List<Token>();     

			var l = new Lexan();
			int y = 0;

			Identifiers.Clear();
			Literals.Clear();

            State state = State.S; // Начальное состояние
            string buffer = ""; // Буфер для накопления символов
            int repeatState = 1; // Флаг для повторной обработки состояния     

            for (int i = 0; i < inputString.Length; i++)
			{
				char c = inputString[i];   // Текущий символ

                if (repeatState == 1) repeatState = 0;     // Сброс флага повторной обработки

                for (int j = -1; j < repeatState; ++j)
				{
					switch (state)
					{
						case State.S:
							if (IsEnglishLetter(c))
                            {    // Переход в состояние идентификатора
                                state = State.I;
								buffer += c;
							}
							else if (IsHexDigitStart(inputString, i))
                            {     // Переход в состояние шестнадцатеричного числа
                                state = State.H;
								buffer += c;
							}
							else if (IsDecimalDigit(c))
                            {     // Переход в состояние десятичного числа
                                state = State.D;
								buffer += c;
							}
							else if (IsWhiteSpace(c)) { }   // Игнорирование пробелов
                            else if (SingleRazdel.Contains(c.ToString()))
                            {     // Добавление одиночного разделителя
                                Tokens.Add(new Token("R", c.ToString(), Array.IndexOf(SingleRazdel, c.ToString())));
							}
							else if (ParnRazdel.Contains(c.ToString()))
                            {     // Переход в состояние разделителя, в случае, если он парный     
                                state = State.R;
								buffer += c;
							}
							else
							{
								l.Error(101, listBox3, c); // Передаем неизвестный символ в обработчик ошибок       
                                y++;
							}

							break;

						case State.I:
							if (IsLetterOrDigit(c))
							{
								if (buffer.Length < 16)
								{
									buffer += c;
								}

                            }    // Накопление символов идентификатора
                            else
							{
								if (SlWord.Contains(buffer))
								{
									Tokens.Add(new Token("W", buffer, Array.IndexOf(SlWord, buffer)));
								}
								else
								{
									if (!Identifiers.Contains(buffer))
									{
										Identifiers.Add(buffer);
									}
									Tokens.Add(new Token("I", buffer, Identifiers.IndexOf(buffer)));
								}
								buffer = "";
								state = State.S;
								repeatState = 1;
							}
							break;


                        case State.H:
                            if (buffer == "0" && (c == 'x' || c == 'X')) { buffer += c; }
                            else if (IsHexDigit(c)) { buffer += c; }
                            else
                            {
                                // Проверка размера шестнадцатеричного числа
                                if (buffer.StartsWith("0x") || buffer.StartsWith("0X"))
                                {
                                    string hexValue = buffer.Substring(2);
                                    if (hexValue.Length > 8)
                                    {
                                        l.Error(102, listBox3, null); // Шестнадцатеричное число слишком большое
                                        y++;
                                    }
                                    else if (hexValue.Length > 4 && hexValue.Length <= 8)
                                    {
                                        // 4-байтное число - допустимо
                                        if (!Literals.Contains(buffer))
                                        {
                                            Literals.Add(buffer);
                                        }
                                        Tokens.Add(new Token("L", buffer, Literals.IndexOf(buffer)));
                                    }
                                    else if (hexValue.Length <= 4)
                                    {
                                        // 2-байтное число - допустимо
                                        if (!Literals.Contains(buffer))
                                        {
                                            Literals.Add(buffer);
                                        }
                                        Tokens.Add(new Token("L", buffer, Literals.IndexOf(buffer)));
                                    }
                                }
                                buffer = "";
                                state = State.S;
                                repeatState = 1;
                            }
                            break;


                        case State.D:
                            if (IsDecimalDigit(c)) { buffer += c; }
                            else
                            {
                                // Проверка размера десятичного числа
                                if (ulong.TryParse(buffer, out ulong decimalValue))
                                {
                                    if (decimalValue > 4294967295)
                                    {
                                        l.Error(103, listBox3, null); // Десятичное число слишком большое
                                        y++;
                                    }
                                    else if (decimalValue > 65535 && decimalValue <= 4294967295)
                                    {
                                        // 4-байтное число - допустимо
                                        if (!Literals.Contains(buffer))
                                        {
                                            Literals.Add(buffer);
                                        }
                                        Tokens.Add(new Token("L", buffer, Literals.IndexOf(buffer)));
                                    }
                                    else if (decimalValue <= 65535)
                                    {
                                        // 2-байтное число - допустимо
                                        if (!Literals.Contains(buffer))
                                        {
                                            Literals.Add(buffer);
                                        }
                                        Tokens.Add(new Token("L", buffer, Literals.IndexOf(buffer)));
                                    }
                                }
                                buffer = "";
                                state = State.S;
                                repeatState = 1;
                            }
                            break;


                        case State.R:
							if (TwoRazdel.Contains(buffer + c))
                            {    // Накопление парного разделителя
                                buffer += c;
							}
							else
							{
                                // Добавление парного разделителя
                                if (buffer.Length > 1)
									Tokens.Add(new Token("R", buffer, Array.IndexOf(TwoRazdel, buffer) + 16));
								else
									Tokens.Add(new Token("R", buffer, Array.IndexOf(ParnRazdel, buffer) + 9));
								buffer = "";
								state = State.S;
								repeatState = 1;
							}
							break;
					}
				}
			}
            // Добавление последнего накопленного буфера, если он не пустой
            if (buffer != "")
			{
				switch (state)
				{
					case State.I:
						if (SlWord.Contains(buffer))
						{
							Tokens.Add(new Token("W", buffer, Array.IndexOf(SlWord, buffer)));
						}
						else
						{
							if (!Identifiers.Contains(buffer))
							{
								Identifiers.Add(buffer);
							}
							Tokens.Add(new Token("I", buffer, Identifiers.IndexOf(buffer)));
						}
						break;
					case State.H:
					case State.D:
						if (!Literals.Contains(buffer))
						{
							Literals.Add(buffer);
						}
						Tokens.Add(new Token("L", buffer, Literals.IndexOf(buffer)));
						break;

					case State.R:
						if (buffer.Length > 1)
							Tokens.Add(new Token("R", buffer, Array.IndexOf(TwoRazdel, buffer) + 16));
						else
							Tokens.Add(new Token("R", buffer, Array.IndexOf(ParnRazdel, buffer) + 9));
						break;
				}
			}
			buffer = "$";
			Tokens.Add(new Token("Z", buffer, Tokens.Count));   // Добавили символ конца строки

            if (y == 0)
			{
				l.Error(-1, listBox3);
			}

			return Tokens;    // Возвращаем список токенов
        }
	}
}