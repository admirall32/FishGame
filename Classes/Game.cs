using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FishGame
{
    class Game
    {
        private List<Player> players;
        private Dictionary<Values, Player> books;
        Deck stock;
        private TextBox textBoxOnForm;

        public Game(string playerName, IEnumerable<string> opponentNames, TextBox textBoxOnForm)
        {
            Random random = new Random();
            this.textBoxOnForm = textBoxOnForm;
            players = new List<Player>();
            players.Add(new Player(playerName, random, textBoxOnForm));

            foreach (string player in opponentNames)
            {
                players.Add(new Player(player, random, textBoxOnForm));
            }

            books = new Dictionary<Values, Player>();
            stock = new Deck();
            Deal();
            players[0].SortHand();
        }
        private void Deal()
        {
            stock.Shuffle();

            for (int i = 0; i < 5; i++)
                foreach (Player player in players)
                    player.TakeCard(stock.Deal());

            foreach (Player player in players)
            {
                player.PullOutBooks();
            }          

            //Именно здесь начинается игра.       
            //Тасуется колода, раздается по пять карт каждому игроку, затем с помощью      
            //цикла foreach вызывается метод PullOutBooks() для каждого игрока.    
        }    
        public bool PlayOneRound(int selectedPlayerCard) 
        {
            Values cardToAskFor = players[0].Peek(selectedPlayerCard).Value; 
                
            for (int i = 0; i < players.Count; i++) 
            { 
                if (i == 0) 
                    players[0].AskForACard(players, 0, stock, cardToAskFor); 
                else 
                    players[i].AskForACard(players, i, stock); 
                    
                if (PullOutBooks(players[i])) 
                { 
                    textBoxOnForm.Text += players[i].Name + " drew a new hand" 
                        + Environment.NewLine; int card = 1; 
                        
                    while (card <= 5 && stock.Count > 0) 
                    { 
                        players[i].TakeCard(stock.Deal()); 
                        card++; 
                    } 
                    
                } 
                    
                players[0].SortHand(); 
                    
                if (stock.Count == 0) 
                { 
                    textBoxOnForm.Text = "The stock is out of cards. Game over!" 
                        + Environment.NewLine; return true; 
                } 
            }
                
            return false;
                //Сыграйте один раз. Параметром является выбранная игроком карта из имеющихся на руках
                //Вызовите метод AskForACard() для каждого из игроков, начиная с человека       
                //снулевым индексом. Затем вызовите метод PullOutBooks() —       
                //если он вернет значение true, значит, у игрока кончились       
                //карты. Закончив со всеми игроками, отсортируйте карты       
                //человека (чтобы список в форме выглядел красиво). Проверьте, не закончились       
                //ли карты в запасе. В случае положительного результатаочистите поле TextBox     
                //и выведите фразу"The stock is out of cards. Game over!".    
        }   
        public bool PullOutBooks(Player player) 
        {
            foreach (Values book in player.PullOutBooks())
            {
                books.Add(book, player);
            }

            if (player.CardCount == 0)
            {
                return true;
            }
            return false;
            //       Игроки выкладывают взятки. Метод возвращает значение true, если карты      
            //       уигрока закончились. Каждая взятка добавляется в словарь Books.   
        }    
        public string DescribeBooks() 
        {
            string returnString = "";
            foreach(KeyValuePair<Values, Player> book in books)
            {
                returnString += book.Value.Name + " has a book of " + Card.Plural(book.Key) + "\r\n";
            }

            return returnString;
            //       Этот метод возвращает длинную строку с описанием взяток каждого игрока,       
            //       взяв за основу содержание словаря Books: "Joe has a book of sixes.     
            //       (перенос строки) Ed has a book of Aces."    
        }
        public string GetWinnerName()
        {
            Dictionary<string, int> winners = new Dictionary<string, int>();
            string resultString = "";
            int maxBooks = 0;
            int winnersCount = 0;

            foreach (Values value in books.Keys)
            {
                if (winners.ContainsKey(books[value].Name))
                {
                    winners[books[value].Name]++;

                    if (winners[books[value].Name] > maxBooks)
                    {
                        maxBooks = winners[books[value].Name];
                    }
                }
                else
                {
                    winners.Add(books[value].Name, 1);
                }
            }            

            foreach (KeyValuePair<string, int> value in winners)
            {
                if (value.Value > maxBooks)
                {
                    maxBooks = value.Value;
                }
            }

            

            foreach (KeyValuePair<string, int> value in winners)
            {
                if (value.Value == maxBooks)
                {
                    resultString += value.Key + " and ";
                    winnersCount++;
                }                
            }
            
            if (winnersCount == 1)
                resultString = "The winner is " + resultString;
            else
                resultString = "A tie between " + resultString;

            resultString = resultString.Substring(0, resultString.Length - 4);

            return resultString + " with " + maxBooks + " books.\r\n";
            
            //       Этот метод вызывается в конце игры. Он использует собственный словарь       
            // (Dictionary<string, int> winners) для отслеживания количества взяток       
            //       каждого игрока. Сначала циклforeach (Values value in books.Keys)      
            //       заполняет словарьwinnersинформацией о взятках. Затем      
            //       словарь просматривается на предмет поиска максимального      
            //       количества взяток. Напоследок словарь просматривается еще один раз, чтобы      
            //       сформировать список победителей в виде строки ("Joe and Ed"). Если победитель  
            //       один, возвращается строка"Ed with 3 books". В противном      
            //       случае возвращается строка"A tie between Joe and Bob with 2 books."
        }

        public IEnumerable<string> GetPlayerCardNames() 
        {
            for (int i = 0; i < players.Count; i++)
            {
                players[i].SortHand();
                //Console.Clear();
                Console.WriteLine(players[i].Name); 
                Console.WriteLine();

                foreach (string str in players[i].GetCardNames())
                {
                    Console.WriteLine(str);
                }

                Console.WriteLine();
            }

            return players[0].GetCardNames(); 
        }
        public string DescribePlayerHands() 
        { 
            string description = ""; 

            for (int i = 0; i < players.Count; i++) 
            {
                description += players[i].Name + " has " + players[i].CardCount; 
                
                if (players[i].CardCount == 1) 
                    description += " card." + Environment.NewLine;        
                else 
                    description += " cards." + Environment.NewLine; 
            } 

            description += "The stock has " + stock.Count + " cards left."; 
            return description; 
        }
    }    
}
