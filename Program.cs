using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lesson_1_AdoNet
{
    class Program
    {
        static void Main(string[] args)
        {
            int number_query=0, index, size; string total_query;
            SqlConnection connectToLibrary = new SqlConnection();
            //connectToLibrary.ConnectionString = "Data Source=SERG-PC\\SQLEXPRESS;Initial Catalog=Library;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
            connectToLibrary.ConnectionString = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;
            connectToLibrary.Open();
            string[] queries = { @"select distinct last_name as lastname, first_name as firstname 
                                    from student inner join s_cards on student.id = s_cards.id_student 
                                    where s_cards.date_in is null 
                                    union 
                                    select distinct last_name as lastname, first_name as firstname 
                                    from teacher inner join t_cards on teacher.id = t_cards.id_teacher 
                                    where t_cards.date_in is null; ",

                                  @"select Author.last_name as Author_lastname, Author.first_name as Author_firstname 
                                    from Author inner join Book on Author.id = Book.id_author 
                                    where Book.id = 3;",

                                  @"select Book.name as Book_name 
                                    from Book 
                                    where Book.quantity > 0;",

                                  @"select Book.name as Book_name 
                                    from Book inner join S_Cards on Book.id = S_Cards.id_book 
                                    where S_Cards.id_student = 2 and S_Cards.date_in is null 
                                    union 
                                    select Book.name as Book_name 
                                    from Book inner join T_Cards on Book.id = T_Cards.id_book 
                                    where T_Cards.id_teacher = 2 and T_Cards.date_in is null;",

                                  @"select Book.name as Book_title
                                    from Book inner join S_Cards on Book.id = S_Cards.id_book
                                    where (datediff (day, S_Cards.date_out, getdate())) <= 14
                                    union all
                                    select Book.name as Book_title
                                    from Book inner join T_Cards on Book.id = T_Cards.id_book
                                    where (datediff (day, T_Cards.date_out, getdate())) <= 14;",

                                  @"update S_Cards set date_in = (convert(varchar(10), getdate(),105))
                                    where date_in is null",

                                  @"select count(S_Cards.id_student)  as Count_books
                                    from S_Cards 
                                    where S_Cards.id_student = 3 and (datediff (day, S_Cards.date_out, getdate())) <= 365;"};

            string text1 = "\n1-debtors, 2-book #3 authors, 3-free books, 4-books in reader #2, ";
            string text2 = "5 - books for 2 last weeks, 6 - reset debts to zero, 7 - count books";
  

            do
            {

                Console.WriteLine(text1); Console.WriteLine(text2);

                do
                {
                    Console.Write("\nEnter the query packet size:\none query packet     - 1\ntwo-queries packet   - 2\nthree-queries packet - 3\n\nsize = ");
                    size = Convert.ToInt32(Console.ReadLine());
                    if (size < 1 || size > 3)
                        Console.WriteLine("Incorrect! Try again\n");
                } while (size < 1 || size > 3);
                Console.WriteLine("----------------------------------------------------------");

                List<int> numbers = new List<int>();

                for (int i = 0; i < size; i++)
                {
                    do
                    {
                        Console.Write("\nEnter number of your query (from 1 to 7): ");
                        number_query = Convert.ToInt32(Console.ReadLine());
                        if (number_query < 1 || number_query > 7 || numbers.Contains(number_query))
                            Console.WriteLine("Incorrect! Try again\n");
                        
                    } while (number_query < 1 || number_query > 7 || numbers.Contains(number_query));
                    numbers.Add(number_query);
                }

                total_query = "";
                for (int i = 0; i < size; i++)
                {
                    total_query = total_query + queries[numbers[i] - 1] + " ";
                }
                //Console.WriteLine(total_query);
                Console.WriteLine("==========================================================");

                Query(connectToLibrary, total_query);

 
                // продовжити ?																																					
                Console.Write("\n\nDo you want to continue? ('1' for 'yes'): ");
                index = Convert.ToInt32(Console.ReadLine());

            } while (index == 1);

            connectToLibrary?.Close();
        }

        private static void Query(SqlConnection connectToLibrary, string total_query)
        {
            SqlCommand cmd1 = new SqlCommand();
            cmd1.Connection = connectToLibrary;
            cmd1.CommandText = total_query;


            SqlDataReader reader = cmd1.ExecuteReader();
            int index = 0;
            do
            {
                while (reader.Read())
                {
                    if (index == 0)
                    {
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            Console.Write(reader.GetName(i) + "\t");
                        }
                        Console.WriteLine("\n----------------------------------------------------------");
                    }

                    index++;
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        Console.Write(reader[i] + "\t");
                    }
                    Console.WriteLine();
                }
                //Console.WriteLine("Обработано записей: " + line.ToString());
                Console.WriteLine("==========================================================\n\n");
                index = 0;
            } while (reader.NextResult());
 
            reader.Close();
        }
    }
}
