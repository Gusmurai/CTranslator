﻿using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.IO;

namespace task1
{
    public partial class Form1 : Form
    {
        List<Token> tokens;
        private SintacsisLR sintacsisLR;
        public Form1()
        {
            InitializeComponent();

        }
        private void AnalyzeText()
        {
            // Очистка ListView перед добавлением новых данных
            listView1.Items.Clear();
            listBox1.Items.Clear(); // Очистка списка ошибок
            listBox2.Items.Clear(); // Очистка списка ошибок
            listBox3.Items.Clear(); // Очистка списка ошибок
            listView2.Items.Clear();
            listBox4.Items.Clear(); // Очистка для генерации кода
            // Получаем текст из textBox1
            string text = textBox1.Text;


            // Обработка текста с помощью лексического анализатора
            var tokens = Lexan.InterpString(text, listBox3); // Получаем список токенов  

            int j = 0;
            foreach (var token in tokens)    // Перебираем каждый токен
            {// Добавляем токен и его классификацию в ListView
                var item = new ListViewItem(token.Value);  // Лексема

                item.SubItems.Add(token.ToString());    // Классификация (тип, номер)      
                listView1.Items.Add(item);

                item.SubItems.Add(j.ToString());
                j++;
            }

            // Выполнение синтаксического анализа
            sintacsisLR = new SintacsisLR(tokens, listBox1, listBox2, listBox4);


            List<Quartet> generatedMatrix = sintacsisLR.GetLogicalMatrix();

            foreach (var quadruple in generatedMatrix)
            {
                var item = new ListViewItem(quadruple.Operator);
                item.SubItems.Add(quadruple.Operand1);
                item.SubItems.Add(quadruple.Operand2);
                item.SubItems.Add(quadruple.Result);

                listView2.Items.Add(item);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            AnalyzeText();    // Выполняем лексический анализ
        }

        private void button2_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "Выберите файл для анализа";
            openFileDialog.Filter = "Текстовые файлы (*.txt)|*.txt|Все файлы (*.*)|*.*";

            if (openFileDialog.ShowDialog() == DialogResult.OK)    
            {
                try
                {// Чтение всего текста из файла
                    string text = File.ReadAllText(openFileDialog.FileName);
                    // Отображение содержимого файла в textBox1
                    textBox1.Text = text;

                    // Выполняем лексический анализ после загрузки файла
                    AnalyzeText();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при загрузке файла: " + ex.Message,
                                  "Ошибка", MessageBoxButtons.OK,
                                  MessageBoxIcon.Error);
                }
            }
           
        }
    }
}