using System;
using System.Threading;

namespace philosophers_asymmetric
{
    class Fork
    {
        private Mutex m = new Mutex();
        public int Id { get; private set; }

        public Fork(int id) { this.Id = id; }

        public void take() { m.WaitOne(); }
        public bool tryTake() { return m.WaitOne(0); }
        public void put()
        {
            try { m.ReleaseMutex(); } catch (ApplicationException) { }
        }
    }

    class Philosopher
    {
        int id;
        Fork fork_left;
        Fork fork_right;
        uint eat_count;
        double wait_time;
        DateTime wait_start;
        bool stop_flag;
        bool debug_flag;
        Random random;

        void think()
        {
            if (this.debug_flag)
            {
                Console.WriteLine(this.id + " thinking");
            }

            Thread.Sleep(1);

            if (this.debug_flag)
            {
                Console.WriteLine(this.id + " hungry");
            }

            this.wait_start = DateTime.Now;
        }

        void eat()
        {
            this.wait_time += DateTime.Now.Subtract(this.wait_start).TotalMilliseconds;
            if (this.debug_flag)
            {
                Console.WriteLine(this.id + " eating");
            }

            Thread.Sleep(1);

            eat_count++;
        }

        public Philosopher(int number, Fork left, Fork right, bool dbg)
        {
            this.id = number;
            this.fork_left = left;
            this.fork_right = right;
            this.eat_count = 0;
            this.wait_time = 0;
            this.debug_flag = dbg;
            this.stop_flag = false;
            this.random = new Random();
        }

        public void run()
        {
            while (!stop_flag)
            {
                think();
                if ((this.id % 2) == 0)
                {
                    fork_left.take();
                    if (debug_flag) Console.WriteLine(this.id + " took left");
                    fork_right.take();
                    if (debug_flag) Console.WriteLine(this.id + " took right");
                }
                else
                {
                    fork_right.take();
                    if (debug_flag) Console.WriteLine(this.id + " took right");
                    fork_left.take();
                    if (debug_flag) Console.WriteLine(this.id + " took left");
                }
                eat();
                if ((this.id % 2) == 0)
                {
                    fork_right.put();
                    fork_left.put();
                }
                else
                {
                    fork_left.put();
                    fork_right.put();
                }

                if (debug_flag) Console.WriteLine(this.id + " put both forks");
            }
        }

        public void stop()
        {
            stop_flag = true;
        }

        public void printStats()
        {
            Console.WriteLine(this.id + " " + this.eat_count + " " + Convert.ToInt32(this.wait_time));
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            int N = 5;
            bool dbg = false;
            int duration = 60000;

            // Создаём вилки с передачей id — это исправляет ошибку CS7036
            Fork[] forks = new Fork[N];
            for (int i = 0; i < N; i++)
            {
                forks[i] = new Fork(i);
            }

            Philosopher[] phils = new Philosopher[N];
            for (int i = 0; i < N; i++)
            {
                phils[i] = new Philosopher(i + 1, forks[(i + 1) % N], forks[i], dbg);
            }

            Thread[] runners = new Thread[N];
            for (int i = 0; i < N; i++)
            {
                runners[i] = new Thread(phils[i].run);
            }
            for (int i = 0; i < N; i++)
            {
                runners[i].Start();
            }

            Thread.Sleep(duration);

            for (int i = 0; i < N; i++)
            {
                phils[i].stop();
            }

            for (int i = 0; i < N; i++)
            {
                runners[i].Join();
            }

            for (int i = 0; i < N; i++)
            {
                phils[i].printStats();
            }
        }
    }
}
