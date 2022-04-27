using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Resources;

namespace VideoPoker
{
    public partial class VideoPoker : Form
    {

        private List<string> deck = new List<string>();
        private List<string> hand = new List<string>();

        private List<bool> lockedCards = new List<bool> {false, false, false, false, false};
        private bool firstDeal = true;

        private int credits = 100;
        private int bet = 1;

        public VideoPoker()
        {
            InitializeComponent();
            InitializeWinTable();
            DisableLockButtons();

            pb_card1.SizeMode = PictureBoxSizeMode.StretchImage;
            pb_card2.SizeMode = PictureBoxSizeMode.StretchImage;
            pb_card3.SizeMode = PictureBoxSizeMode.StretchImage;
            pb_card4.SizeMode = PictureBoxSizeMode.StretchImage;
            pb_card5.SizeMode = PictureBoxSizeMode.StretchImage;

            btn_deal.BackColor = Color.Green;
            btn_bet.BackColor = Color.Blue;

            lbl_credits_amount.Text = string.Format("{0}€", credits);
        }

        private void MakeDeck()
        {
            deck.Clear();
            hand.Clear();

            string[] values = { "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12", "13" };
            string[] suits = { "c", "d", "h", "s" };
            string temp_card = "";

            foreach (string suit in suits)
            {
                for (int i = 0; i < values.Length; i++)
                {
                    temp_card = string.Format("{0}_{1}", values[i], suit);
                    deck.Add(temp_card);
                }
            }

            Random rand = new Random();
            deck = deck.OrderBy(c => rand.Next()).ToList();
       
            FirstDeal();
        }

        private void FirstDeal()
        {
            for (int i = 0; i < 5; i++)
            {
                lockedCards[i] = false;
            }

            for (int i = 1; i < 6; i++)
            {
                firstDeal = false;
                string card = deck[i];
                hand.Add(deck[i]);
                
                ResourceManager resManager = new ResourceManager("VideoPoker.Properties.Resources", GetType().Assembly);
                Image myImage = (Image)(resManager.GetObject(card));

                string pb_name = string.Concat("pb_card", i.ToString());
                PictureBox pb = (PictureBox)this.Controls.Find(pb_name, true)[0];

                pb.Image = myImage;   
            }
        }
        private void LockCards()
        {
            int x = 1;

            for (int i = 0; i < 5; i++)
            {
                if (!lockedCards[i])
                {
                    hand[i] = deck[5 + x];

                    ResourceManager resManager = new ResourceManager("VideoPoker.Properties.Resources", GetType().Assembly);
                    Image myImage = (Image)(resManager.GetObject(deck[5 + x]));

                    string pb_name = string.Concat("pb_card", (i + 1).ToString());
                    PictureBox pb = (PictureBox)this.Controls.Find(pb_name, true)[0];

                    pb.Image = myImage;
                    x++;
                }
            }

            
            firstDeal = true;

            SortCards(hand);
        }

        private List<int> sortedValues = new List<int>();
        private List<string> sortedSuits = new List<string>();
        string[] temp = new string[20];
        private void SortCards(List<string> hand)
        {
            sortedValues.Clear();
            sortedSuits.Clear();
            Array.Clear(temp, 0, temp.Length);

            for (int i = 0; i < hand.Count; i++)
            {
                string[] temp = hand[i].Split('_');
                sortedValues.Add(Convert.ToInt32(temp[0]));
                sortedSuits.Add(temp[1].ToString());
            }

            sortedValues.Sort();
            CheckWins();
        }

        private void CheckWins()
        {
            btn_deal.Enabled = false;

            if (isFlush(sortedSuits) && isStraight(sortedValues))
            {
                lbl_sflush.ForeColor = Color.Gold;
                lbl_sflush_win.ForeColor = Color.Gold;

                credits += (bet * 1000);
                lbl_credits_amount.Text = string.Format("{0}€", credits);
            }

            else if (isFlush(sortedSuits))
            {
                lbl_flush.ForeColor = Color.Gold;
                lbl_flush_win.ForeColor = Color.Gold;

                credits += (bet * 5);
                lbl_credits_amount.Text = string.Format("{0}€", credits);
            }

            else if (isStraight(sortedValues))
            {
                lbl_straight.ForeColor = Color.Gold;
                lbl_straight_win.ForeColor = Color.Gold;

                credits += (bet * 3);
                lbl_credits_amount.Text = string.Format("{0}€", credits);
            }

            else if (isFours(sortedValues))
            {
                lbl_fkind.ForeColor = Color.Gold;
                lbl_fkind_win.ForeColor = Color.Gold;

                credits += (bet * 50);
                lbl_credits_amount.Text = string.Format("{0}€", credits);
            }

            else if (isFullHouse(sortedValues))
            {
                lbl_fhouse.ForeColor = Color.Gold;
                lbl_fhouse_win.ForeColor = Color.Gold;

                credits += (bet * 10);
                lbl_credits_amount.Text = string.Format("{0}€", credits);
            }

            else if (isThrees(sortedValues))
            {
                lbl_tkind.ForeColor = Color.Gold;
                lbl_tkind_win.ForeColor = Color.Gold;

                credits += (bet * 2);
                lbl_credits_amount.Text = string.Format("{0}€", credits);
            }

            else if (isTwoPairs(sortedValues))
            {
                lbl_tpairs.ForeColor = Color.Gold;
                lbl_tpairs_win.ForeColor = Color.Gold;

                credits += (bet * 1);
                lbl_credits_amount.Text = string.Format("{0}€", credits);
            }

            else
            {
                if (credits == 0)
                {
                    MessageBox.Show("Game over! Quitting game..");
                    Application.Exit();
                }
            }

            InitializeButtons();
        }

        private bool isFlush(List<string> suits)
        {
            if (suits[0] == suits[1] &&
                suits[1] == suits[2] &&
                suits[2] == suits[3] && 
                suits[3] == suits[4])
            {
                return true;
            }

            else
            {
                return false;
            }
        }

        private bool isStraight(List<int> values)
        {
            if (values[0] == 1 &&
                values[1] == 10 &&
                values[2] == 11 &&
                values[3] == 12 &&
                values[4] == 13)
            {
                return true;
            }

            else if (values[0] == values[1] - 1 &&
                    values[1] == values[2] - 1 &&
                    values[2] == values[3] - 1 &&
                    values[3] == values[4] - 1)
            {
                return true;
            }

            else
            {
                return false;
            }

        }

        private bool isFours(List<int> values)
        {
            if (values[0] == values[1] &&
                values[1] == values[2] &&
                values[2] == values[3] &&
                values[3] == values[4])
            {
                return true;
            }

            else
            {
                return false;
            }
        }

        private bool isFullHouse(List<int> values)
        {
            if ((values[0] == values[1] &&
                values[2] == values[3] &&
                values[2] == values[4]) ||

                (values[0] == values[1] &&
                values[0] == values[2] &&
                values[3] == values[4]))
            {
                return true;
            }

            else
            {
                return false;
            }
        }

        private bool isThrees(List<int> values)
        {
            if ((values[0] == values[1] && 
                values[1] == values[2]) ||

                (values[1] == values[2] &&
                values[2] == values[3]) ||

                (values[2] == values[3] &&
                values[3] == values[4]))
            {
                return true;
            }

            else
            {
                return false;
            }
        }

        private bool isTwoPairs(List<int> values)
        {
            if ((values[0] == values[1] &&
                values[2] == values[3]) ||

                (values[0] == values[1] &&
                values[3] == values[4]) ||

                (values[1] == values[2] &&
                values[3] == values[4]))
            {
                return true;
            }

            else
            {
                return false;
            }
        }

        private void InitializeButtons()
        {
            DisableLockButtons();

            btn_lock1.Text = "Lock Card";
            btn_lock2.Text = "Lock Card";
            btn_lock3.Text = "Lock Card";
            btn_lock4.Text = "Lock Card";
            btn_lock5.Text = "Lock Card";

            btn_bet.Enabled = true;
            btn_deal.Enabled = true;
        }

        private void InitializeWinTable()
        {
            lbl_sflush.ForeColor = Color.Black;
            lbl_sflush_win.ForeColor = Color.Black;
            lbl_sflush_win.Text = string.Format("{0}€", (bet * 1000));

            lbl_fkind.ForeColor = Color.Black;
            lbl_fkind_win.ForeColor = Color.Black;
            lbl_fkind_win.Text = string.Format("{0}€", (bet * 50));

            lbl_fhouse.ForeColor = Color.Black;
            lbl_fhouse_win.ForeColor = Color.Black;
            lbl_fhouse_win.Text = string.Format("{0}€", (bet * 10));

            lbl_flush.ForeColor = Color.Black;
            lbl_flush_win.ForeColor = Color.Black;
            lbl_flush_win.Text = string.Format("{0}€", (bet * 5));

            lbl_straight.ForeColor = Color.Black;
            lbl_straight_win.ForeColor = Color.Black;
            lbl_straight_win.Text = string.Format("{0}€", (bet * 3));

            lbl_tkind.ForeColor = Color.Black;
            lbl_tkind_win.ForeColor = Color.Black;
            lbl_tkind_win.Text = string.Format("{0}€", (bet * 2));

            lbl_tpairs.ForeColor = Color.Black;
            lbl_tpairs_win.ForeColor = Color.Black;
            lbl_tpairs_win.Text = string.Format("{0}€", (bet * 1));
        }
        private void EnableLockButtons()
        {
            btn_lock1.Enabled = true;
            btn_lock2.Enabled = true;
            btn_lock3.Enabled = true;
            btn_lock4.Enabled = true;
            btn_lock5.Enabled = true;

            btn_lock1.BackColor = Color.Red;
            btn_lock2.BackColor = Color.Red;
            btn_lock3.BackColor = Color.Red;
            btn_lock4.BackColor = Color.Red;
            btn_lock5.BackColor = Color.Red;
        }

        private void DisableLockButtons()
        {
            btn_lock1.Enabled = false;
            btn_lock2.Enabled = false;
            btn_lock3.Enabled = false;
            btn_lock4.Enabled = false;
            btn_lock5.Enabled = false;

            btn_lock1.BackColor = Color.DarkRed;
            btn_lock2.BackColor = Color.DarkRed;
            btn_lock3.BackColor = Color.DarkRed;
            btn_lock4.BackColor = Color.DarkRed;
            btn_lock5.BackColor = Color.DarkRed;
        }

        private void btn_deal_Click(object sender, EventArgs e)
        {
            if (firstDeal)
            {
                if (credits >= bet)
                {
                    credits -= bet;
                    lbl_credits_amount.Text = string.Format("{0}€", credits);

                    btn_bet.Enabled = false;
                    InitializeWinTable();
                    EnableLockButtons();
                    MakeDeck();
                }

                else
                {
                    MessageBox.Show("Not enough credits!");
                }
            }

            else 
            {
                DisableLockButtons();
                LockCards();
            }
        }


        private void btn_lock1_Click(object sender, EventArgs e)
        {
            if (lockedCards[0] == true)
            {
                lockedCards[0] = false;
                btn_lock1.Text = "Lock Card";
            }

            else
            {
                lockedCards[0] = true;
                btn_lock1.Text = "Locked";
            }
        }

        private void btn_lock2_Click(object sender, EventArgs e)
        {
            if (lockedCards[1] == true)
            {
                lockedCards[1] = false;
                btn_lock2.Text = "Lock Card";
            }

            else
            {
                lockedCards[1] = true;
                btn_lock2.Text = "Locked";
            }
        }

        private void btn_lock3_Click(object sender, EventArgs e)
        {
            if (lockedCards[2] == true)
            {
                lockedCards[2] = false;
                btn_lock3.Text = "Lock Card";
            }

            else
            {
                lockedCards[2] = true;
                btn_lock3.Text = "Locked";
            }
        }

        private void btn_lock4_Click(object sender, EventArgs e)
        {
            if (lockedCards[3] == true)
            {
                lockedCards[3] = false;
                btn_lock4.Text = "Lock Card";
            }

            else
            {
                lockedCards[3] = true;
                btn_lock4.Text = "Locked";
            }
        }

        private void btn_lock5_Click(object sender, EventArgs e)
        {
            if (lockedCards[4] == true)
            {
                lockedCards[4] = false;
                btn_lock5.Text = "Lock Card";
            }

            else
            {
                lockedCards[4] = true;
                btn_lock5.Text = "Locked";
            }
        }

        private void btn_bet_Click(object sender, EventArgs e)
        {
            if (bet < 5)
            {
                bet++;
                lbl_bet_amount.Text = string.Format("{0}€", bet);
                InitializeWinTable();
            }

            else
            {
                bet = 1;
                lbl_bet_amount.Text = string.Format("{0}€", bet);
                InitializeWinTable();
            }
        }
    }
}
