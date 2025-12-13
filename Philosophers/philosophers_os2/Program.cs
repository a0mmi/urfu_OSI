using System;
using System.Threading;

namespace philosophers_semaphore {
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

        // Shared semaphore: разрешает одновременно пытаться взять вилки не более N-1 философам
        public static SemaphoreSlim room = null;

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

        public void run()
        {
            while (!stop_flag)
            {
                think();

                // Семофор: запросить разрешение войти (максимум N-1 одновременно)
                room.Wait();
                try
                {
                    // После получения разрешения — взять обе вилки (блокирующие вызовы)
                    fork_left.take();
                    if (debug_flag) Console.WriteLine(this.id + " took left");
                    fork_right.take();
                    if (debug_flag) Console.WriteLine(this.id + " took right");
                }
                finally
                {
                    // освободить семафор сразу после того, как обе вилки взяты,
                    // чтобы другие философы могли пытаться захватить вилки
                    room.Release();
                }

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

            Console.WriteLine("Mode: semaphore (N-1). N={0}, duration={1} ms, debug={2}", N, duration, dbg);

            // инициализация семафора (максимум N-1)
            Philosopher.room = new SemaphoreSlim(N - 1, N - 1);

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