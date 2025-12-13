using System;
using System.Threading;

namespace philosophers_waiter {
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
        int id; // 1..N
        Fork fork_left;
        Fork fork_right;
        public uint eat_count;
        double wait_time; // ms
        DateTime wait_start;
        volatile bool stop_flag;
        bool debug_flag;
        Random random;

        // waiterLock служит "официантом" — монитором, который синхронизирует попытки взять обе вилки
        public static object waiterLock = new object();

        public Philosopher(int number, Fork left, Fork right, bool dbg)
        {
            this.id = number;
            this.fork_left = left;
            this.fork_right = right;
            this.eat_count = 0;
            this.wait_time = 0;
            this.debug_flag = dbg;
            this.stop_flag = false;
            this.random = new Random(number * 997);
        }

        void think()
        {
            if (debug_flag) Console.WriteLine(this.id + " thinking");
            Thread.Sleep(1);
            if (debug_flag) Console.WriteLine(this.id + " hungry");
            this.wait_start = DateTime.Now;
        }

        void eat()
        {
            this.wait_time += DateTime.Now.Subtract(this.wait_start).TotalMilliseconds;
            if (debug_flag) Console.WriteLine(this.id + " eating");
            Thread.Sleep(1);
            eat_count++;
        }

        // Вариант с официантом: философ просит официанта "разрешить" взять вилки.
        // Официант берёт монопольную блокировку, проверяет/пытается взять вилки неблокирующим способом:
        // либо философ получает обе вилки сразу, либо не получает ни одной (и офиц. отпускает).
        void AcquireWithWaiter()
        {
            while (!stop_flag)
            {
                lock (waiterLock) {
                    // Попытаться неблокирующе взять левую вилку
                    bool leftTaken = fork_left.tryTake();
                    if (!leftTaken) {
                        // не получилось — официант отпускает и философ будет пробовать позже
                    } else {
                        // левую взяли — попытаться взять правую
                        bool rightTaken = fork_right.tryTake();
                        if (!rightTaken)
                        {
                            // если правую взять не удалось, освободить левую и retry
                            fork_left.put();
                        }
                        else
                        {
                            // обе вилки взяты — уходим с ними
                            if (debug_flag) Console.WriteLine(this.id + " took left & right (via waiter)");
                            return;
                        }
                    }
                    // если не удалось получить обе вилки — выйдем из lock и попробуем снова позже
                }
                // небольшой бэкофф (чтобы не крутить горячую петлю)
                Thread.Sleep(1);
            }
        }

        public void run()
        {
            while (!stop_flag)
            {
                think();

                // запросить у официанта взять вилки
                AcquireWithWaiter();

                eat();

                // вернуть вилки
                fork_right.put();
                fork_left.put();
                if (debug_flag) Console.WriteLine(this.id + " put right & left");
            }
        }

        public void stop() { stop_flag = true; }

        public void printStats() { Console.WriteLine(this.id + " " + this.eat_count + " " + Convert.ToInt32(this.wait_time)); }
    }

    class Program
    {
        static void Main(string[] args)
        {
            int N = 5;
            int duration = 60000;
            bool dbg = false;

            if (args.Length >= 1) int.TryParse(args[0], out N);
            if (args.Length >= 2) int.TryParse(args[1], out duration);
            if (args.Length >= 3) bool.TryParse(args[2], out dbg);

            if (N <= 1) { Console.WriteLine("N must be >= 2"); return; }

            Console.WriteLine("Mode: waiter (arbiter). N={0}, duration={1} ms, debug={2}", N, duration, dbg);

            // создать вилки
            Fork[] forks = new Fork[N];
            for (int i = 0; i < N; i++) forks[i] = new Fork(i);

            Philosopher[] phils = new Philosopher[N];
            for (int i = 0; i < N; i++)
            {
                Fork left = forks[(i + 1) % N];
                Fork right = forks[i];
                phils[i] = new Philosopher(i + 1, left, right, dbg);
            }

            Thread[] runners = new Thread[N];
            for (int i = 0; i < N; i++) runners[i] = new Thread(phils[i].run);
            for (int i = 0; i < N; i++) runners[i].Start();

            Thread.Sleep(duration);

            for (int i = 0; i < N; i++) phils[i].stop();
            for (int i = 0; i < N; i++) runners[i].Join();

            Console.WriteLine("Results:");
            for (int i = 0; i < N; i++) phils[i].printStats();
        }
    }
}
