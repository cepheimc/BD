using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using Faker;                  //библиотека для наполнения таблиц данными


namespace lab2
{
    class Program
    {
        static int number;
        static void Main(string[] args)
        {

            string connectionString = @"Data Source=I100\SQLEXPRESS;Initial Catalog=Composed_CD;Integrated Security=True";  //подключаем бд
            SqlConnection cn = new SqlConnection(connectionString);
            cn.Open();
            ConsoleKeyInfo cki;

            do
            {
                Console.WriteLine("Выберите действие:\n1.Очистить таблицу \n2.Добавить количество строк \n3.Заполнить таблицу\n4.Выполнить запрос без индекса \n5.Выполнить запрос с индексом \n");
                int change = int.Parse(Console.ReadLine());
                switch (change)
                {
                    case 1:                           //действие очистить таблицу
                        {
                            SqlCommand command1 = new SqlCommand("DELETE FROM Client", cn);    //запрос для удаления данных с таблицы
                            SqlDataReader reader1 = command1.ExecuteReader();
                            reader1.Close();
                            Console.WriteLine("Таблица пустая!");
                            break;
                        }
                    case 2:                     //действие добавления количества строк
                        {
                            Console.Write("Введите количество строк: ");
                            number = int.Parse(Console.ReadLine());
                            if (number <= 0)
                            {
                                Console.WriteLine("Количество строк <= 0");
                                break;
                            }
                            break;
                        }
                    case 3:                        //действие заполнения таблицы
                        {
                            System.Diagnostics.Stopwatch myStopwatch = new System.Diagnostics.Stopwatch();
                            myStopwatch.Start();                         //включаем таймер
                            if (number <= 0)
                            {
                                Console.WriteLine("Количество строк <= 0");
                                break;
                            }
                            else
                            {
                                Random r = new Random();
                                // Console.WriteLine(number);
                                SqlDataReader reader1, reader2;
                                for (int i = 0; i < number; i++)
                                {
                                    SqlCommand command1 = new SqlCommand("INSERT INTO Client(ClID, LastName, FirstName, Country, City, AddressC, Company) VALUES (@id, @last, @name, @country, @city, @addr, @comp)", cn);   //запрос на заполнение таблицы
                                    command1.Parameters.Add("@id", i + 1);
                                    command1.Parameters.Add("@last", Faker.NameFaker.LastName());
                                    command1.Parameters.Add("@name", Faker.NameFaker.FirstName());
                                    command1.Parameters.Add("@country", Faker.LocationFaker.Country());
                                    command1.Parameters.Add("@city", Faker.LocationFaker.City());
                                    command1.Parameters.Add("@addr", Faker.LocationFaker.Street());
                                    command1.Parameters.Add("@comp", Faker.CompanyFaker.Name());
                                    reader1 = command1.ExecuteReader();             //выполенение запроса
                                    reader1.Close();
                                }
                                for (int i = 0; i < number; i++)
                                {
                                    SqlCommand command2 = new SqlCommand("INSERT INTO OrderClient(OrderID, OrderDate, ClID, ProgrID, TypeID) VALUES (@id, @date, @id1, @id2, @id3)", cn);  //запрос на заполнение таблицы с внешними ключами
                                    command2.Parameters.Add("@id", i + 1);
                                    command2.Parameters.Add("@date", Faker.DateTimeFaker.DateTime());
                                    command2.Parameters.Add("@id1", r.Next(1, number));
                                    command2.Parameters.Add("@id2", r.Next(1, 12));
                                    command2.Parameters.Add("@id3", r.Next(1, 6));
                                    reader2 = command2.ExecuteReader();
                                    reader2.Close();
                                }
                            }
                            myStopwatch.Stop();                 //останавливаем таймер
                            Console.WriteLine("Время выполения запроса: {0} milliseconds", myStopwatch.ElapsedMilliseconds.ToString());
                            break;
                        }
                    case 4:                             //выполнения запроса без индекса с where, join
                        {
                            System.Diagnostics.Stopwatch myStopwatch = new System.Diagnostics.Stopwatch();
                            myStopwatch.Start();                   //запескаем таймер
                            SqlCommand command1 = new SqlCommand("SELECT t1.FirstName, t1.LastName, t2.OrderDate FROM Client AS t1 JOIN OrderClient AS t2 ON t1.ClID = t2.ClID WHERE t1.FirstName LIKE 'A%'", cn);
                            SqlDataReader reader1 = command1.ExecuteReader();        //выполняем выше указанный запрос
                            reader1.Close();
                            myStopwatch.Stop();              //останавливаем таймер
                            Console.WriteLine("Время выполения запроса: {0} milliseconds", myStopwatch.ElapsedMilliseconds.ToString());
                            break;
                        }
                    case 5:                   //выполнение запроса с индексом и where, join
                        {
                            SqlCommand command1 = new SqlCommand("CREATE INDEX index1 ON Client (FirstName)", cn); //создаем индексы
                            SqlDataReader reader1 = command1.ExecuteReader();
                            reader1.Close();
                            System.Diagnostics.Stopwatch myStopwatch = new System.Diagnostics.Stopwatch();
                            myStopwatch.Start();               //включаем таймер
                            SqlCommand command2 = new SqlCommand("SELECT t1.FirstName, t1.LastName, t2.OrderDate FROM Client AS t1 JOIN OrderClient AS t2 ON t1.ClID = t2.ClID WHERE t1.FirstName LIKE 'A%'", cn);
                            SqlDataReader reader2 = command2.ExecuteReader(); //выполняем выше указанный запрос
                            reader2.Close();
                            myStopwatch.Stop();       //останавливаем таймер
                            SqlCommand command3 = new SqlCommand("DROP INDEX index1 ON Client", cn);  //удаляем индексы
                            SqlDataReader reader3 = command3.ExecuteReader();
                            reader3.Close();
                            Console.WriteLine("Время выполения запроса: {0} milliseconds", myStopwatch.ElapsedMilliseconds.ToString());
                            break;
                        }
                }
                Console.WriteLine("Продолжить - enter Выход = esc");
                cki = Console.ReadKey();
            } while (cki.Key != ConsoleKey.Escape);
            cn.Close();

        }
    }
}

