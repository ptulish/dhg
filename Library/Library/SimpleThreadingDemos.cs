namespace Library;

public class SimpleThreadingDemos
{
    private int counter = 0;

    public void CounterFunc()
    {
        while (counter < 50)
        {
            Console.WriteLine($"Thread {Thread.CurrentThread.ManagedThreadId} counts from {counter} to {++counter}");
            Thread.Sleep(100);
        }
    }

    internal static void RunThreadsDemo()
    {
        Console.WriteLine($"RunThreadDemo started in thread {Thread.CurrentThread.ManagedThreadId}");
        
        //sample 1: single-thread exec
        // SimpleThreadingDemos std = new SimpleThreadingDemos();
        // std.CounterFunc();
        
        //sample 2: multi-threaded exec
        SimpleThreadingDemos std = new SimpleThreadingDemos();
        Thread threadA = new Thread(new ThreadStart(std.CounterFunc));
        threadA.Start();
        Thread threadB = new Thread(new ThreadStart(std.CounterFunc));
        threadB.Start();
        Thread threadC = new Thread(() =>
            {
                std.CounterFunc();
            }
            );
        threadC.Start();
        
    }   
    
}

