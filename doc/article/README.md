# Life inside `await`

Let's consider an asynchronous call placed in an `async` method:

```csharp
public async Task DoWork()
{
	await Task.Delay(1000);
}
```

Our main question for today is:

- What is the `await` keyword doing behind the scene? Is there anything worth talking about?

Actually, it does many things, too many to be discussed in a single Pill. We will consider just a few of them today.

## Analyse the `async` behaviour

The intuition tells us that the `await` keyword is a syntactic sugar for some more complex code. Let's take it step-by-step.

### 1) Replace `async` with `return`

**Behaviour**

- The `await` is actually returning a promise, a `Task` instance, that the caller can use to be noticed when the asynchronous work was finished.

**Code Transformation**

- Ok, let's replace the `await` with a simple `return`.

```csharp
public Task DoWork()
{
	return Task.Delay(1000);
}
```

**Problem**

- This can work with that initial method that does not have any other code after the `await`, but what happens if there is additional code placed after the `await`?

```csharp
public async Task DoWork()
{
	// before await
    Console.WriteLine("before await");

    // await call
    await Task.Delay(1000);

    // after await
    Console.WriteLine("after await");
}
```

Replacing the `await` with `return` will prevent the additional code to be executed. We must find a way to still execute it.

### 2) Use `ContinueWith` to execute the Continuation

**Behaviour**

- The code after `await` must be executed only after the initial asynchronous call (the delay in our example) is finished.
- The caller must be notified only after this continuation is finished, too.

**Code Transformation**

Luckily we already have the `ContinueWith` method that we can use.

- First, let's enclose the code after the `await` in a local function (we call it `Continuation`)
- Call this local function in a `ContinueWith` method.

```csharp
public async Task DoWork()
{
	// before await
    Console.WriteLine("before await");

    // await call
    return Task.Delay(1000).ContinueWith(t => {
        Continuation();
    });

    void Continuation()
    {
        // after await
        Console.WriteLine("after await");
    }
}
```

The `ContinueWith` method takes the initial `Task` as parameter and returns a new `Task` instance that can be returned to the caller.

**Note**: The code before the `await` is not a problem. It is executed before the awaited call ether if we use `await` or `return`.

### 3) What about the `SynchronizationContext`?

**Behaviour**

If a `SynchronizationContext` instance is configured for the current thread, the `await` is capturing it at the beginning and it is used at the end to execute the continuation in that context.

> TBD: screenshot and examples are needed to highlight this behaviour.

Let's do the same ourselves.

**Code Transformation**

```csharp
public async Task DoWork()
{
	// before await
    Console.WriteLine("before await");

    // await call
    SynchronizationContext capturedSynchronizationContext = SynchronizationContext.Current;

    return Task.Delay(1000).ContinueWith(t =>
    {
        if (capturedSynchronizationContext == null)
        {
            Continuation();
        }
        else
        {
            capturedSynchronizationContext.Post(state => Continuation(), null);
        }
    });

    void Continuation()
    {
        // after await
        Console.WriteLine("after await");
    }
}
```

**Problem**

- When a `SynchronizationContext` is configured on the current thread, the continuation is not really awaited. We need a mechanism to wait until it is finished.

### Use `ManualResetEventSlim` to wait until Continuation is done

```csharp
// async call

SynchronizationContext capturedSynchronizationContext = SynchronizationContext.Current;

return Task.Delay(1000).ContinueWith(t =>
{
    if (capturedSynchronizationContext == null)
    {
        Continuation();
    }
    else
    {
        ManualResetEventSlim manualResetEventSlim = new ManualResetEventSlim(false);

        void SendOrPostCallback(object state)
        {
            Continuation();
            manualResetEventSlim.Set();
        }

        capturedSynchronizationContext.Post(SendOrPostCallback, null);

        manualResetEventSlim.Wait();
    }
});
```

**Note**: We reproduced the behaviour, but I am not sure how the `async-await` mechanism really awaits for the continuation to finish. In the code

.NET source code:

- https://referencesource.microsoft.com/#mscorlib/system/threading/Tasks/TaskContinuation.cs,d8b8d04cc476b392,references

