using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Tab;


/*public static class GlobalVariables
{
    public static float GlobX { get; set; }
    public static float GlobY { get; set; }
    public static int flagDrag { get; set; }
}*/

















namespace WindowsFormsApp3
{
    public partial class Form2 : Form
    {
        System.Windows.Forms.Timer tt = new System.Windows.Forms.Timer();
        DateTime currentTime = DateTime.Now;
        int mouse_x;
        int mouse_y;
        int flag_click = 0;
        int click_x = 0;
        int click_y = 0;
        Form1 f1;
        float globX = 0;
        float globY = 0;
        string globS = null;

        string globString = "";
        private UdpClient server;
        private Thread listenerThread;
        private void StartServer()
        {
            listenerThread = new Thread(new ThreadStart(ListenForClients));
            listenerThread.Start();
        }
        private volatile bool isRunning = true;
        private void StopServer()
        {
            try
            {
                // Close the server socket
                isRunning = false;
                server?.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }
        private void ListenForClients()
        {
            try
            {
                server = new UdpClient(12345);
                Console.WriteLine("Server started...");

                IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);

                while (isRunning)
                {
                    byte[] receivedBytes = server.Receive(ref remoteEndPoint);
                    string clientMessage = Encoding.ASCII.GetString(receivedBytes);

                    // Handle client message
                    HandleClientMessage(clientMessage, remoteEndPoint);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
        int verafied = 0;
        string userName = "";
        string emo = "";
        float age = 0;

        private void HandleClientMessage(string clientMessage, IPEndPoint clientEndPoint)
        {
            Console.WriteLine($"Received from {clientEndPoint}: {clientMessage}");
            if (clientMessage[2] == 'e')
            {
                int i = 7;
                string s = "";

                for (; i < clientMessage.Length; i++)
                {

                    if (clientMessage[i] == '\'')
                    {
                        break;
                    }
                    s += clientMessage[i];

                }
                emo = s;

            }
            if (clientMessage[2] == 'S')
            {
                int i = 7;
                string s = "";

                for (; i < clientMessage.Length; i++)
                {

                    if (clientMessage[i] == '\'')
                    {
                        break;
                    }
                    s += clientMessage[i];

                }

                globS = s;

            }
            if (clientMessage[2] == 'v')
            {
                verafied = 1;
                int i = 20;
                for(; clientMessage[i]!='\'';i++)
                {

                    userName += clientMessage[i];
                }
                user_name=userName;
                string a = "";
                a += userName[4];
                a += userName[5];
                age = float.Parse(a, CultureInfo.InvariantCulture.NumberFormat);

            }
            else if (clientMessage[2] == 'U' || clientMessage[2] == 'L')
            {
                if (clientMessage[2] == 'U')
                {
                    flag_click = 0;
                }
                else
                {
                    flag_click = 1;
                }
                Console.WriteLine(flag_click);
                string s = "";
                int i = 6;
                for (; i < clientMessage.Length; i++)
                {

                    if (clientMessage[i] == ',')
                    {
                        break;
                    }
                    s += clientMessage[i];

                }

                float num = float.Parse(s, CultureInfo.InvariantCulture.NumberFormat);

                num = 1 - num;

                num = num * this.Width;
                double x = num - globX;
                x = Math.Pow(x, 2);


                i++;
                s = "";
                for (; i < clientMessage.Length; i++)
                {

                    if (clientMessage[i] == ']')
                    {
                        break;
                    }
                    s += clientMessage[i];

                }

                float numTwo = float.Parse(s, CultureInfo.InvariantCulture.NumberFormat);
                numTwo = (numTwo * this.Height);

                double y = numTwo - globY;
                y = Math.Pow(y, 2);


                double def = x + y;
                def = Math.Sqrt(def);
                float delta;
                if (def > 10)
                {
                    delta = num - globX;
                    globX += delta;
                    delta = numTwo - globY;
                    globY += delta;
                }
                s = "";
            }



            // You can add your logic for processing the received message here
        }
        private void Form2_FormClosing(object sender, FormClosingEventArgs e)
        {
            StopServer();
        }

        

        public Form2()
        {

            this.WindowState = FormWindowState.Maximized;
            Load += Form1_Load;
            tt.Tick += Tt_Tick;
            tt.Interval = 1000 / 60;
            tt.Start();
            this.FormClosing += Form2_FormClosing;
        }




        //form1//////////////////////////////////////////////////////////
        int flag_english_vocabulary = 0;
        int flag_compose_sentences = 0;
        int flag_check_spelling = 0;
        Bitmap button = new Bitmap("buton.png");
        Bitmap english_vocabulary = new Bitmap("english_vocabulary.png");
        Bitmap english_vocabulary_hover = new Bitmap("english_vocabulary_hover.png");
        Bitmap compose_sentences_hover = new Bitmap("compose_sentences_hover.png");
        Bitmap check_spelling_hover = new Bitmap("check_spelling2_hover.png");
        Bitmap compose_sentences = new Bitmap("compose_sentences.png");
        Bitmap check_spelling = new Bitmap("check_spelling2.png");
        Bitmap games = new Bitmap("games.png");
        /////////////////////////////////////////////////////////////////////////
        //from2//////////////////////////////////////////////////////////////////







        int flagfors = 0;
        class line
        {
            public int x1, y1, x2, y2, flag;
            public string name1;
            public string name2;
            public Color clr = Color.Black;
        }
        class word
        {
            public int x, y, w, h;
            public string word_name;
            public Bitmap img;
        }
        class food
        {
            public int x, y, w, h;
            public string food_name;
            public Bitmap img;
        }

        List<line> lline = new List<line>();
        int flag_line = 0;
        line pline = new line();
        int ct_scoure = 1;

        string str = null;
        int flag_scoure = 0;
        List<food> lfood = new List<food>();
        List<word> lwords = new List<word>();
        Bitmap back_icon = new Bitmap("back_icon.png");
        Bitmap buton = new Bitmap("buton.png");
        Bitmap scoure_img = null;
        Font font1 = new Font("Arfmoochikncheez", 43);
        Font font2 = new Font("Arfmoochikncheez", 40);
        Pen pen = new Pen(Color.Black, 10);
        Bitmap scoure_img2 = new Bitmap("play_agin.png");
        Bitmap scoure_img3 = new Bitmap("next.png");
        float scoure = 0.0f;
        int ct = 0;
        public void chick_game()
        {
            scoure = 0;
            for (int i = 0; i < lline.Count; i++)
            {
                if (lline[i].name1 == lline[i].name2)
                {
                    scoure++;
                    lline[i].clr = Color.ForestGreen;
                }
                else
                {
                    lline[i].clr = Color.Red;
                }
            }

        }
        public void create_food(string[] foodArray)
        {
            int start_x = 200;
            for (int i = 0; i < foodArray.Length; i++)
            {
                food pnn = new food();
                pnn.food_name = foodArray[i];
                pnn.img = new Bitmap(foodArray[i] + ".png");
                pnn.x = start_x;
                pnn.y = 180;
                pnn.w = 230;
                pnn.h = 200;
                lfood.Add(pnn);
                start_x += 330;
            }
        }
        public void ShuffleArray<T>(T[] array)
        {
            Random random = new Random();

            // Fisher-Yates shuffle algorithm
            for (int i = array.Length - 1; i > 0; i--)
            {
                int j = random.Next(0, i + 1);
                // Swap elements
                T temp = array[i];
                array[i] = array[j];
                array[j] = temp;
            }
        }
        public void create_words(string[] wordsArray)
        {
            int start_x = 150;
            for (int i = 0; i < wordsArray.Length; i++)
            {
                word pnn = new word();
                pnn.x = start_x;
                pnn.y = 600;
                pnn.w = 300;
                pnn.h = 100;
                pnn.word_name = wordsArray[i];
                pnn.img = new Bitmap("buton.png");
                start_x += 330;
                lwords.Add(pnn);
            }
        }

        /// /////////////////////////////////////////////////////////////////////
        //form3///////////////////////////////////////////////////////////////////
        int flag_word = -1;
        int delta_x, delta_y;
        int ctt = 0;
        int old_x, old_y;
        int flag_save = 0;
        int ct_drop = 0;
        int obj_x = 0;
        string[] sentence1;
        string[] sentence1_check;
        int obj_y = 0;
        float final_scoure = 0.0f;
        int scoure2 = 0;
        class word_form3
        {
            public int x, y, w, h;
            public string word_name;
            public Bitmap img;
        }
        List<word_form3> lwords_form3 = new List<word_form3>();
        class reg
        {
            public int x, y, w, h;
            public string word_name;
            public string drobed_name;
            public Color clr = Color.White;
            public int done = 0;

        }
        List<reg> lreg = new List<reg>();
        Brush brush = Brushes.White;
        Font font = new Font("Arfmoochikncheez", 35);

        public void check_end()
        {
            scoure = 0;
            for (int i = 0; i < lreg.Count; i++)
            {
                if (lreg[i].word_name == lreg[i].drobed_name)
                {
                    lreg[i].clr = Color.Green;
                    scoure++;
                }
                else
                {
                    lreg[i].clr = Color.Red;
                }
            }


        }
        public void check_in()
        {
            int flag_check = 0;
            for (int i = 0; i < lreg.Count; i++)
            {
                if (mouse_x >= lreg[i].x && mouse_x <= lreg[i].x + lreg[i].w)
                {
                    if (mouse_y >= lreg[i].y && mouse_y <= lreg[i].y + lreg[i].h)
                    {
                        lwords_form3[flag_word].x = lreg[i].x + 5;
                        lwords_form3[flag_word].y = lreg[i].y + 5;
                        lreg[i].clr = Color.Yellow;
                        lreg[i].drobed_name = lwords_form3[flag_word].word_name;
                        lreg[i].done = 1;
                        ct_drop++;
                        flag_check = 1;
                    }
                }
            }
            if (flag_check == 0)
            {
                lwords_form3[flag_word].x = old_x;
                lwords_form3[flag_word].y = old_y;
            }
        }
        public void create_reg(string[] sentence1, string[] sentence1_check)
        {
            int start_x = 150;
            int start_y = 460;
            for (int i = 0; i < sentence1.Length; i++)
            {
                reg pnn = new reg();
                pnn.x = start_x;
                pnn.y = start_y;
                pnn.w = 310;
                pnn.h = 110;
                start_x += 330;
                pnn.word_name = sentence1_check[i];
                lreg.Add(pnn);
                if (i == 3)
                {
                    start_y += 140;
                    start_x = 150;
                }
            }
        }
        public void create_words_form3(string[] sentence1)
        {
            //int start = this.Width/;
            int start_x = 150;
            int start_y = 180;
            for (int i = 0; i < sentence1.Length; i++)
            {
                word_form3 pnn = new word_form3();
                pnn.x = start_x;
                pnn.y = start_y;
                pnn.w = 300;
                pnn.h = 100;
                pnn.word_name = sentence1[i];
                pnn.img = new Bitmap("buton.png");
                start_x += 330;
                if (i == 3)
                {
                    start_y += 120;
                    start_x = 150;
                }
                lwords_form3.Add(pnn);
            }
        }

        int ver = 0;

        int countThree = 1;
        /// ///////////////////////////////////////////////////////////////////// <summary>
        /// /////////////////////////////////////////////////////////////////////
        /// 
        /// 
        ///         
        int scoure_form4 = 0;
        int flag_stop = 1;
        int flag_scoure4 = 0;
        int ct4 = 1;
        string show_word = null;
        int ct_input = 0;
        string randomElement = null;
        Bitmap scoure_img4 = null;
        string input_voice = null;
        int ct_animate = 0;
        int flag_an = 0;
        Bitmap mic = new Bitmap("mic.png");
        Font font4 = new Font("Arfmoochikncheez", 65);
        Bitmap specker = new Bitmap("specker.png");
        void animate_voice()
        {
            string[] words1 = input_voice.ToLower().Split();
            if (show_word != input_voice)
            {

            }
            show_word += words1[ct_input];
            if (show_word == input_voice)
            {
                flag_an = 1;
            }
            else
            {
                show_word += " ";
            }
            ct_input += 1;

        }
        static int chick_form4(string str1, string str2)
        {
            // Split the strings into words and normalize to lowercase
            string[] words1 = str1.ToLower().Split();
            string[] words2 = str2.ToLower().Split();

            // Find the minimum length to avoid index errors
            int minLength = Math.Min(words1.Length, words2.Length);

            // Count the matching words at the same positions
            int matchingCount = 0;
            for (int i = 0; i < minLength; i++)
            {
                if (words1[i] == words2[i])
                {
                    matchingCount += 1;
                }
            }

            return matchingCount;

        }
        void get_scoure(int ss)
        {
            string[] words1 = randomElement.ToLower().Split();
            string[] words2 = input_voice.ToLower().Split();
            int rr = Math.Max(words1.Length, words2.Length);
            double scoure2 = ((double)ss / rr) * 100;

            this.Text = "" + scoure_form4;
            if (scoure2 >= 0 && scoure2 < 25)
            {
                scoure_form4 = 1;
            }
            if (scoure2 >= 25 && scoure2 < 50)
            {
                scoure_form4 = 2;
            }
            if (scoure2 >= 50 && scoure2 < 75)
            {
                scoure_form4 = 3;
            }
            if (scoure2 >= 75 && scoure2 <= 100)
            {
                scoure_form4 = 4;
            }
            //this.Text = "" + ss + "/" + rr;

        }
        void Reset_game()
        {
            scoure_form4 = 0;
            flag_stop = 1;
            flag_scoure4 = 0;
            globS = null;
            ct4 = 1;
            randomElement = null;
            scoure_img4 = null;
            input_voice = null;
            ct_animate = 0;
            flag_an = 0;
            ct_input = 0;
            show_word = null;
            List<string> elements = new List<string> { "what is your name", "how are you today", "can you help me", "let's play together", "i go to school every day" };
            Random random = new Random();
            int randomIndex = random.Next(elements.Count);
            randomElement = elements[randomIndex];

        }
        /// 
        /// 
        /// 
        /// 
        /// 
        /// </summary>

        private void Tt_Tick(object sender, EventArgs e)
        {
            if(verafied==1)
            {
                countThree++;
                if (countThree% 53==0)
                {
                    ver = 1;
                }
            }
            
            if (ver == 1)
            {
                mouse_x = (int)globX;
                click_x = mouse_x;
                mouse_y = (int)globY;
                click_y = mouse_y;


                if (flag_form2 == 0 && flag_form3 == 0 )
                {
                    tic_form1();
                }

                if (flag_form2 == 1)
                {
                    if (flag_creat_form2 == 0)
                    {
                        creat_form2();
                        flag_creat_form2 = 1;
                    }
                    tic_form2();
                }
                if (flag_form3 == 1)
                {
                    if (flag_creat_form3 == 0)
                    {
                        creat_form3();
                        flag_creat_form3 = 1;

                    }
                    tic_form3();
                }
                if (flag_form4 == 1)
                {
                    if (flag_creat_form4 == 0)
                    {
                        creat_form4();
                        flag_creat_form4 = 1;
                    }
                    tic_form4();
                }
            }
            this.Text = mouse_x+"";
            DrawDubb(this.CreateGraphics());
        }
        int flag_form2 = 0;
        int flag_form3 = 0;
        int flag_form4 = 0;
        int flag_creat_form2 = 0;
        int flag_creat_form3 = 0;
        int flag_creat_form4=0;

        void creat_form4()
        {
            List<string> elements = new List<string> { "what is your name", "how are you today", "can you help me", "let's play together", "i go to school every day" };
            Random random = new Random();
            int randomIndex = random.Next(elements.Count);
            randomElement = elements[randomIndex];
        }
        public void creat_form3()
        {
            if (age < 10)
            {
                sentence1 = new string[] { "Jane", "borrows", "Richard's", "eraser" };
                sentence1_check = new string[] { "Jane", "borrows", "Richard's", "eraser" };
            }
            else
            {   //If I’m late, I’ll call you
                sentence1 = new string[] { "If", "I’m", "late", "I’ll", "call", "you" };
                sentence1_check = new string[] { "If", "I’m", "late", "I’ll", "call", "you" };
                sentence1 = new string[] { "If", "I", "became", "president", "I", "would", "reduce", "taxes" };
                sentence1_check = new string[] { "If", "I", "became", "president", "I", "would", "reduce", "taxes" };
            }


            ShuffleArray(sentence1);
            create_words_form3(sentence1);
            create_reg(sentence1, sentence1_check);
        }
        public void creat_form2()
        {
            flag_scoure = 0;
            string[] foodArray;
            string[] wordsArray;
            if (6 <= currentTime.Hour && currentTime.Hour < 18)
            {
                foodArray = new string[] { "egg", "orange", "waffle", "Strawberry" };
                wordsArray = new string[] { "egg", "orange", "waffle", "Strawberry" };
            }
            else
            {
                foodArray = new string[] { "pizza", "pasta", "Shrimp", "salad" };
                wordsArray = new string[] { "pizza", "pasta", "Shrimp", "salad" };
            }
            ShuffleArray(foodArray);
            create_food(foodArray);


            ShuffleArray(wordsArray);
            create_words(wordsArray);
        }

        public void tic_form4()
        {
            input_voice = globS;
            if (flag_stop == 1 && input_voice != null)
            {
                if (!string.Equals(show_word, input_voice, StringComparison.OrdinalIgnoreCase) && ct_animate % 5 == 0 && flag_an == 0)
                {
                    animate_voice();
                }
                else
                {
                    if (ct4 % 40 == 0)
                    {
                        int ss = chick_form4(input_voice, randomElement);
                        get_scoure(ss);
                        flag_scoure4 = 1;
                    }
                    ct4 += 1;
                }
                ct_animate += 1;
            }
            if (flag_click == 1)
            {
                if (flag_scoure4 == 1)
                {
                    if (click_x >= this.Width / 2 - 150 && click_x <= (this.Width / 2 - 150) + 290)
                    {
                        if (click_y >= 530 && click_y <= 530 + 100)
                        {
                            Reset_game();
                        }
                    }
                }
                //10, 10, 100, 110
                if (click_x >= 10 && click_x <= 110)
                {
                    if (click_y >= 10 && click_y <= 120)
                    {
                        Reset_game();
                        flag_form4 = 0;
                    }
                }
            }
        }

        int flag_lock = 0;
        public void tic_form3()
        {
            if (flag_scoure == 0)
            {
                if (flag_click == 1)
                {

                    for (int i = 0; i < lwords_form3.Count; i++)
                    {
                        if (flag_lock == 0)
                        {
                            if (click_x >= lwords_form3[i].x && click_x <= lwords_form3[i].x + lwords_form3[i].w)
                            {
                                if (click_y >= lwords_form3[i].y && click_y <= lwords_form3[i].y + lwords_form3[i].h)
                                {
                                    flag_lock = 1;
                                    if (flag_save == 0)
                                    {
                                        old_x = lwords_form3[i].x;
                                        old_y = lwords_form3[i].y;
                                    }
                                    flag_save++;
                                    flag_word = i;
                                }
                            }
                        }
                    }
                    if (flag_word != -1)
                    {
                        if (ctt != 0)
                        {
                            obj_x = lwords_form3[flag_word].x;
                            obj_y = lwords_form3[flag_word].y;
                            delta_x = mouse_x - lwords_form3[flag_word].x;
                            delta_y = mouse_y - lwords_form3[flag_word].y;
                            lwords_form3[flag_word].x += delta_x - lwords_form3[flag_word].w / 2;
                            lwords_form3[flag_word].y += delta_y - lwords_form3[flag_word].h / 2;
                        }
                        ctt++;
                    }
                }
                else
                {
                    if (flag_word != -1)
                    {
                        check_in();
                        if (ct_drop == sentence1.Length)
                        {
                            check_end();

                        }

                    }
                    if (ct_drop == sentence1.Length)
                    {
                        ct_scoure++;
                        if (ct_scoure % 20 == 0)
                        {
                            flag_scoure = 1;
                        }
                    }
                    flag_word = -1; ctt = 0; flag_save = 0; flag_lock = 0;
                }
            }
            else
            {
                if (flag_click == 1)
                {
                    if (click_x >= this.Width / 2 - 155 && click_x <= this.Width / 2 - 155 + 290)
                    {
                        if (click_y >= 530 && click_y <= 530 + 100)
                        {
                            lwords_form3.Clear();
                            lreg.Clear();
                            Random rr = new Random();
                            int sen = rr.Next(1, 5);
                            if (age < 10)
                            {
                                if (sen == 1)
                                {
                                    sentence1 = new string[] { "The", "shark", "swims", "in", "The", "ocean" };
                                    sentence1_check = new string[] { "The", "shark", "swims", "in", "The", "ocean" };
                                }
                                if (sen == 2)
                                {
                                    sentence1 = new string[] { "Mother", "bought", "a", "new", "bag" };
                                    sentence1_check = new string[] { "Mother", "bought", "a", "new", "bag" };
                                }
                                if (sen == 3)
                                {
                                    sentence1 = new string[] { "where", "are", "you", "from", "my", "friend" };
                                    sentence1_check = new string[] { "where", "are", "you", "from", "my", "friend" };
                                }
                                if (sen == 4)
                                {

                                    sentence1 = new string[] { "We", "will", "have", "a", "picnic", "on", "Sunday" };
                                    sentence1_check = new string[] { "We", "will", "have", "a", "picnic", "on", "Sunday" };
                                }
                            }
                            else
                            {
                                sen = rr.Next(1, 4);
                                if (sen == 1)
                                {
                                    //If I became president, I would reduce taxes
                                    sentence1 = new string[] { "If", "I", "became", "president", "I", "would", "reduce", "taxes" };
                                    sentence1_check = new string[] { "If", "I", "became", "president", "I", "would", "reduce", "taxes" };
                                }
                                if (sen == 2)
                                {
                                    //If I were you, I would study hard
                                    sentence1 = new string[] { "If", "I", "were", "you", "I", "would", "study", "hard" };
                                    sentence1_check = new string[] { "If", "I", "were", "you", "I", "would", "study", "hard" };
                                }
                                if (sen == 3)
                                {
                                    //If you heat ice cream, it melts
                                    sentence1 = new string[] { "If", "you", "heat", "ice", "cream", "it", "melts" };
                                    sentence1_check = new string[] { "If", "you", "heat", "ice", "cream", "it", "melts" };
                                }
                            }
                        



                            ShuffleArray(sentence1);
                            create_words_form3(sentence1);
                            create_reg(sentence1, sentence1_check);
                            ct_scoure = 1;
                            click_x = 0;
                            click_y = 0;
                            flag_scoure = 0;
                            scoure_img = null;
                            scoure = 0;
                            ct_drop = 0;
                            ctt = 0;
                            scoure2 = 0;
                            final_scoure = 0;
                        }
                    }
                }
            }
            if (click_x >= 10 && click_x <= 110)
            {
                if (click_y >= 10 && click_y <= 120)
                {
                    lwords_form3.Clear();
                    lreg.Clear();
                    ct_scoure = 1;
                    click_x = 0;
                    click_y = 0;
                    flag_scoure = 0;
                    scoure_img = null;
                    scoure = 0;
                    ct_drop = 0;
                    ctt = 0;
                    flag_form3 = 0;
                    flag_creat_form3 = 0;
                    scoure2 = 0;
                    final_scoure = 0;
                }
            }
        }
        public void tic_form2()
        {
            click_x = mouse_x;
            click_y = mouse_y;
            if (flag_click == 1 && flag_scoure == 0)
            {

                if (flag_line == 0)
                {

                    for (int i = 0; i < lfood.Count; i++)
                    {
                        if (click_x >= lfood[i].x && click_x <= lfood[i].x + lfood[i].w)
                        {
                            if (click_y >= lfood[i].y && click_y <= lfood[i].y + lfood[i].h)
                            {
                                flag_line += 1;
                                pline.flag = 1;
                                str = lfood[i].food_name;
                            }
                        }
                    }

                    for (int i = 0; i < lwords.Count; i++)
                    {
                        if (click_x >= lwords[i].x && click_x <= lwords[i].x + lwords[i].w)
                        {
                            if (click_y >= lwords[i].y && click_y <= lwords[i].y + lwords[i].h)
                            {
                                pline.flag = 2;
                                flag_line += 1;
                                str = lwords[i].word_name;
                            }
                        }
                    }
                }
                if (flag_line == 1)
                {
                    pline.x1 = click_x;
                    pline.y1 = click_y;
                    pline.name1 = str;
                    flag_line++;
                }


            }
            else
            {

                if (flag_line == 2)
                {
                    if (pline.flag == 2)
                    {
                        for (int i = 0; i < lfood.Count; i++)
                        {
                            if (click_x >= lfood[i].x && click_x <= lfood[i].x + lfood[i].w)
                            {
                                if (click_y >= lfood[i].y && click_y <= lfood[i].y + lfood[i].h)
                                {
                                    flag_line += 1;
                                    str = lfood[i].food_name;
                                }
                            }
                        }
                    }
                    if (pline.flag == 1)
                    {
                        for (int i = 0; i < lwords.Count; i++)
                        {
                            if (click_x >= lwords[i].x && click_x <= lwords[i].x + lwords[i].w)
                            {
                                if (click_y >= lwords[i].y && click_y <= lwords[i].y + lwords[i].h)
                                {
                                    flag_line += 1;
                                    str = lwords[i].word_name;
                                }
                            }
                        }
                    }
                    if (flag_line == 3)
                    {
                        pline.x2 = mouse_x;
                        pline.y2 = mouse_y;
                        pline.name2 = str;
                        lline.Add(pline);
                        pline = new line();
                        flag_line = 0;
                    }
                    else
                    {
                        pline = new line();
                    }

                }
                flag_line = 0;
            }
            if (flag_line == 2)
            {
                pline.x2 = mouse_x;
                pline.y2 = mouse_y;
            }

            ct++;
            if (lline.Count == 4)
            {
                chick_game();
                ct_scoure++;
            }
            if (ct_scoure % 20 == 0)
            {
                flag_scoure = 1;
            }
            if (flag_scoure == 1 && flag_click == 1)
            {
                if (click_x >= this.Width / 2 - 155 && click_x <= this.Width / 2 - 155 + 290)
                {
                    if (click_y >= 530 && click_y <= 530 + 100)
                    {

                        lline.Clear();
                        lfood.Clear();
                        lwords.Clear();
                        string[] foodArray;
                        string[] wordsArray;
                        if (6 <= currentTime.Hour && currentTime.Hour < 18)
                        {
                            foodArray = new string[] { "egg", "orange", "waffle", "Strawberry" };
                            wordsArray = new string[] { "egg", "orange", "waffle", "Strawberry" };
                        }
                        else
                        {
                            foodArray = new string[] { "pizza", "pasta", "Shrimp", "salad" };
                            wordsArray = new string[] { "pizza", "pasta", "Shrimp", "salad" };
                        }
                        ShuffleArray(foodArray);
                        create_food(foodArray);
                        ShuffleArray(wordsArray);
                        create_words(wordsArray);
                        ct_scoure = 1;
                        click_x = 0;
                        click_y = 0;
                        flag_scoure = 0;
                        scoure_img = null;
                        scoure = 0;


                    }
                }
            }
            if (flag_click == 1)
            {
                if (click_x >= 10 && click_x <= 110)
                {
                    if (click_y >= 10 && click_y <= 120)
                    {
                        lline.Clear();
                        lfood.Clear();
                        lwords.Clear();
                        ct_scoure = 1;
                        click_x = 0;
                        click_y = 0;
                        flag_scoure = 0;
                        scoure_img = null;
                        flag_form2 = 0;
                        flag_creat_form2 = 0;
                        scoure = 0;
                    }
                }
            }
        }
        public void tic_form1()
        {
            flag_english_vocabulary = 0;
            flag_compose_sentences = 0;
            flag_check_spelling = 0;

            if (mouse_x >= (this.Width / 2) - 150 && mouse_x <= ((this.Width / 2) - 150) + 300)
            {
                if (mouse_y >= 250 && mouse_y <= 250 + 100)
                {
                    flag_english_vocabulary = 1;
                }
                if (mouse_y >= 375 && mouse_y <= 375 + 100)
                {
                    flag_compose_sentences = 1;
                }
                if (mouse_y >= 500 && mouse_y <= 500 + 100)
                {
                    flag_check_spelling = 1;
                }
            }
            if (flag_click == 1)
            {
                if (mouse_x >= (this.Width / 2) - 150 && mouse_x <= ((this.Width / 2) - 150) + 300)
                {
                    if (mouse_y >= 250 && mouse_y <= 250 + 100)
                    {
                        flag_form2 = 1;
                    }
                    if (mouse_y >= 375 && mouse_y <= 375 + 100)
                    {
                        flag_form3 = 1;
                    }
                    if (mouse_y >= 500 && mouse_y <= 500 + 100)
                    {
                        flag_form4 = 1;
                    }

                }
            }

        }



        public void draw_form4(Graphics g2)
        {
            g2.Clear(Color.SteelBlue);
            g2.DrawImage(button, this.Width / 2 - 225, 10, 450, 150);
            g2.DrawImage(mic, this.Width - 225, 160, 200, 600);
            g2.DrawImage(specker, 5, 520, 150, 150);
            //g2.DrawImage(mic, 300, 900, this.Width - 425, 30);
            g2.DrawString("Speaking test", font, brush, this.Width / 2 - 190, 60);

            //randomElement
            g2.DrawString("say:" + randomElement, font4, brush, 15, 300);
            if (input_voice != null)
            {
                g2.DrawString(show_word, font4, brush, 170, 520);
            }
            if (flag_scoure4 == 1)
            {
                g2.Clear(Color.SteelBlue);
                if (scoure_img4 == null)
                {
                    scoure_img4 = new Bitmap(scoure_form4 + "star.png");
                }

                g2.DrawImage(scoure_img4, this.Width / 2 - 250, 100, 500, 400);
                g2.DrawImage(scoure_img2, this.Width / 2 - 150, 530, 290, 100);
            }
            g2.DrawImage(back_icon, 10, 10, 100, 110);
        }


        public void draw_form3(Graphics g2)
        {
            g2.DrawImage(button, this.Width / 2 - 225, 10, 450, 150);
            g2.DrawString("compose sentence", font, brush, this.Width / 2 - 200, 60);
            for (int i = 0; i < lwords_form3.Count; i++)
            {
                g2.DrawImage(lwords_form3[i].img, lwords_form3[i].x, lwords_form3[i].y, lwords_form3[i].w, lwords_form3[i].h);
                int xx = 0;
                if (lwords_form3[i].word_name.Length < 8)
                {
                    xx = 40;
                }
                if (lwords_form3[i].word_name.Length < 7)
                {
                    xx = 60;
                }
                if (lwords_form3[i].word_name.Length < 4)
                {
                    xx = 90;
                }
                g2.DrawString(lwords_form3[i].word_name, font2, brush, lwords_form3[i].x + xx, lwords_form3[i].y + 20);

            }

            for (int i = 0; i < lreg.Count; i++)
            {
                pen.Color = lreg[i].clr;
                g2.DrawRectangle(pen, lreg[i].x, lreg[i].y, lreg[i].w, lreg[i].h);
            }
            if (flag_scoure == 1)
            {
                g2.Clear(Color.LightSkyBlue);
                if (scoure_img == null)
                {
                    final_scoure = (scoure / sentence1.Length) * 100;
                    if (final_scoure >= 0 && final_scoure < 25)
                    {
                        scoure2 = 1;
                    }
                    if (final_scoure >= 25 && final_scoure < 50)
                    {
                        scoure2 = 2;
                    }
                    if (final_scoure >= 50 && final_scoure < 75)
                    {
                        scoure2 = 3;
                    }
                    if (final_scoure >= 75 && final_scoure <= 100)
                    {
                        scoure2 = 4;
                    }

                    scoure_img = new Bitmap(scoure2 + "star.png");
                }

                g2.DrawImage(scoure_img, this.Width / 2 - 250, 100, 500, 400);
                g2.DrawImage(scoure_img2, this.Width / 2 - 155, 530, 290, 100);

            }
            g2.DrawImage(back_icon, 10, 10, 100, 110);
        }
        public void draw_form2(Graphics g2)
        {
            g2.DrawImage(buton, this.Width / 2 - 225, 10, 450, 150);

            Brush brush = Brushes.White;
            g2.DrawString("matching game", font1, brush, this.Width / 2 - 200, 50);
            for (int i = 0; i < lfood.Count; i++)
            {
                g2.DrawImage(lfood[i].img, lfood[i].x, lfood[i].y, lfood[i].w, lfood[i].h);
            }
            for (int i = 0; i < lwords.Count; i++)
            {
                g2.DrawImage(lwords[i].img, lwords[i].x, lwords[i].y, lwords[i].w, lwords[i].h);
                brush = Brushes.White;
                int xx = 0;
                if (lwords[i].word_name.Length < 8)
                {
                    xx = 40;
                }
                if (lwords[i].word_name.Length < 7)
                {
                    xx = 60;
                }
                if (lwords[i].word_name.Length < 4)
                {
                    xx = 90;
                }
                g2.DrawString(lwords[i].word_name, font2, brush, lwords[i].x + xx, lwords[i].y + 20);

            }

            for (int i = 0; i < lline.Count; i++)
            {
                Pen pen2 = new Pen(lline[i].clr, 10);
                g2.DrawLine(pen2, lline[i].x1, lline[i].y1, lline[i].x2, lline[i].y2);
            }
            if (flag_line == 2)
            {
                g2.DrawLine(pen, pline.x1, pline.y1, pline.x2, pline.y2);
            }
            if (flag_scoure == 1)
            {
                g2.Clear(Color.LightSkyBlue);
                if (scoure_img == null)
                {
                    scoure_img = new Bitmap(scoure + "star.png");
                }

                g2.DrawImage(scoure_img, this.Width / 2 - 250, 100, 500, 400);
                g2.DrawImage(scoure_img2, this.Width / 2 - 155, 530, 290, 100);
                //g2.DrawImage(scoure_img3, this.Width / 2, 530, 290, 100);
            }
            g2.DrawImage(back_icon, 10, 10, 100, 110);
        }
        public void draw_form1(Graphics g2)
        {
            g2.DrawImage(games, (this.Width / 2) - 225, 50, 450, 150);
            if (flag_english_vocabulary == 0)
            {
                g2.DrawImage(english_vocabulary, (this.Width / 2) - 150, 250, 300, 100);
            }
            else
            {
                g2.DrawImage(english_vocabulary_hover, (this.Width / 2) - 150, 250, 300, 100);
            }
            if (flag_compose_sentences == 0)
            {
                g2.DrawImage(compose_sentences, (this.Width / 2) - 150, 375, 300, 100);
            }
            else
            {
                g2.DrawImage(compose_sentences_hover, (this.Width / 2) - 150, 375, 300, 100);
            }
            if (flag_check_spelling == 0)
            {
                g2.DrawImage(check_spelling, (this.Width / 2) - 150, 500, 300, 100);
            }
            else
            {
                g2.DrawImage(check_spelling_hover, (this.Width / 2) - 150, 500, 300, 100);
            }
        }
        public void end_form1()
        {
            button.Dispose();
            button = null;
            english_vocabulary.Dispose();
            english_vocabulary = null;
            english_vocabulary_hover.Dispose();
            english_vocabulary_hover = null;
            compose_sentences_hover.Dispose();
            compose_sentences_hover = null;
            check_spelling_hover.Dispose();
            check_spelling_hover = null;
            compose_sentences.Dispose();
            compose_sentences = null;
            check_spelling.Dispose();
            check_spelling = null;
            games.Dispose();
            games = null;
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            off = new Bitmap(ClientSize.Width, ClientSize.Height);
            StartServer();
        }

        Bitmap sun = new Bitmap("sun.png");
        Bitmap moon = new Bitmap("moon.png");
        Bitmap locked = new Bitmap("locked.png");
        Bitmap UnLocked = new Bitmap("unlocked.png");
        Font ff = new Font("Arial", 15);
        int xA = 210;
        int countTwo = 0;
        Bitmap boy = new Bitmap("boy.png");
        string user_name = "sobeih";
        string hello;
        Font font3 = new Font("Arfmoochikncheez", 35);
        void DrawScene(Graphics g2)
        {
            if (ver == 0)
            {
                countTwo++;
                if (countTwo % 5 == 0)
                {
                    xA += 5;
                }
                g2.Clear(Color.DarkSlateGray);
                g2.FillRectangle(Brushes.Black, 0, 0, 320, 100);

                if (verafied == 0)
                {
                    g2.DrawString("Verifying", font, Brushes.White, 0, 0);
                    g2.DrawString("please keep your camera open", ff, Brushes.White, 10, 65);
                    g2.DrawImage(locked, 600, 200, 350, 400);
                }
                else
                {
                    g2.DrawString("Verafied", font, Brushes.White, 0, 0);
                    g2.DrawString("Hello: " + userName, ff, Brushes.White, 10, 65);
                    g2.DrawImage(UnLocked, 600, 200, 350, 400);

                }
                g2.FillEllipse(Brushes.White, xA, 30, 20, 20);

                if (xA > 280)
                {
                    xA = 210;
                }

            }
            else
            {
                g2.Clear(Color.SteelBlue);

                if (flag_form2 == 0 && flag_form3 == 0 && flag_form4 == 0)
                {
                    hello = "hello " + user_name + " let's \n play together";
                    if (6 <= currentTime.Hour && currentTime.Hour < 18)
                    {
                        g2.DrawImage(sun, this.Width - 320, 10, 300, 300);
                    }
                    else
                    {
                        g2.DrawImage(moon, this.Width - 320, 10, 300, 300);
                    }
                    g2.DrawImage(boy, 10, 200, 500, 500);
                    g2.DrawString(hello, font3, brush, 50, 250);
                    draw_form1(g2);
                }
                else
                {
                    if (flag_form2 == 1)
                    {
                        draw_form2(g2);
                    }
                    if (flag_form3 == 1)
                    {
                        draw_form3(g2);
                    }
                    if (flag_form4 == 1)
                    {
                        draw_form4(g2);
                    }
                }

                if (emo != "")
                {
                    g2.DrawString("you look: "+emo, font3, brush, 900, 20);

                }
                g2.FillEllipse(Brushes.White, globX, globY, 25, 25);
            }
        }
            
        Bitmap off;
        void DrawDubb(Graphics g)
        {
            Graphics g2 = Graphics.FromImage(off);
            DrawScene(g2);
            g.DrawImage(off, 0, 0);
        }
    }
}

