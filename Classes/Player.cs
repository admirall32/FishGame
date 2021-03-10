using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FishGame
{
    class Player
    {
        private string name;
        public string Name { get { return name; } }
        private Random random;
        private Deck cards;
        private TextBox textBoxOnForm;
        public Player(string PlayerName, Random PlayerRandom, TextBox TextBoxOnForm)
        {
            name = PlayerName;
            random = PlayerRandom;
            textBoxOnForm = TextBoxOnForm;
            cards = new Deck(new Card[] { });
            textBoxOnForm.Text = name + " has joined the game \r\n";
        }
        public IEnumerable<Values> PullOutBooks() 
        {
            List<Values> books = new List<Values>(); 
            
            for (int i = 1; i <= 13; i++) 
            { 
                Values value = (Values)i; 
                int howMany = 0; 
                
                for (int card = 0; card < cards.Count; card++) 
                    if (cards.Peek(card).Value == value) 
                        howMany++; 
                
                if (howMany == 4) 
                { 
                    books.Add(value); 
                    cards.PullOutValues(value); 
                } 
            }

            return books;
        }
        public Values GetRandomValue()
        {
            return cards.Peek(random.Next(cards.Count)).Value;
            //Этот метод получает случайное значение, но из числа карт колоды!    
        }    
        public Deck DoYouHaveAny(Values value) 
        {
            Deck deck = cards.PullOutValues(value);
            textBoxOnForm.Text += name + " has " + deck.Count + " " + Card.Plural(value) + "\r\n";
            return deck;
            //Соперник спрашивает о наличии у меня карты нужного достоинства       
            //Используйте метод Deck.PullOutValues() для взятия карт. Добавьте в TextBox       
            //строку′′Joe has 3 sixes′′, используйте новый статический метод Card.Plural()    
        }    
        public void AskForACard(List<Player> players, int myIndex, Deck stock) 
        {
            AskForACard(players, myIndex, stock, GetRandomValue());

            //Это перегруженная версия AskForACard() — выберите случайную карту с помощью       
            //метода GetRandomValue() и спросите о ней методом AskForACard()    
        }    
        public void AskForACard(List<Player> players, int myIndex, Deck stock, Values value) 
        {
            int addedCards = 0;
            textBoxOnForm.Text += players[myIndex].Name + 
                " asks if anyone has a " + value + "\r\n\r\n";

            for (int i = 0; i < players.Count; i++)
            {
                if (i != myIndex)
                {
                    Deck transferDeck;
                    transferDeck = players[i].DoYouHaveAny(value);

                    if (transferDeck.Count > 0)
                    {
                        addedCards += transferDeck.Count;
                        for (int j = 0; j < transferDeck.Count; j++)
                        {
                            players[myIndex].TakeCard(transferDeck.Deal());
                        }
                    }
                }                
            }

            textBoxOnForm.Text += players[myIndex].Name +
            " receive " + addedCards + " " + value + "\r\n\r\n";

            if (addedCards == 0)
            {
                textBoxOnForm.Text += players[myIndex].Name +
                " had to draw from the stock.\r\n\r\n";
                if (stock.Count > 0)
                    cards.Add(stock.Deal(random.Next(stock.Count)));
                else 
                    MessageBox.Show("Stock = 0");

            }

            //Спросите карту у соперников. Добавьте в TextBox текст: ′′Joe asks if anyone has 
            // a Queen′′. В качестве параметра вам будет передана коллекция игроков       
            //спросите (с помощью метода DoYouHaveAny()), есть ли у них карты       
            //указанного достоинства. Переданные им карты добавьте в свой набор.       
            //Следите за тем, сколько карт было добавлено. Если ни одной, вам нужно       
            //взять карту из запаса (передается как параметр), в текстовое       
            //       поле нужно добавить строку TextBox: ′′Joe had to draw from the stock′′    
        }    
        
        // Перечень свойств и коротких методов, которые уже были написаны    
        public int CardCount { get { return cards.Count; } }    
        public void TakeCard(Card card) { cards.Add(card); }    
        public IEnumerable<string> GetCardNames() { return cards.GetCardNames(); }    
        public Card Peek(int cardNumber) { return cards.Peek(cardNumber); }    
        public void SortHand() { cards.SortByValue(); }
    }
    
}
