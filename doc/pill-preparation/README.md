# Life inside `await`

## Preparation Recipe

### Prerequisites

- Create a Console Application with .NET5.

- In the `Main` method, call an asynchronous method (`Task.Delay`) and display context information before and after.

  ```csharp
  private static async Task Main(string[] args)
  {
      // before await
      DisplayInfo("Before await", 0);
  
      // async call
      await Task.Delay(1000);
  
      // after await
      DisplayInfo("After await", 0);
  }
  ```

- Create the helper method `DisplayInfo` that displays information about thread and synchronization context.

  ```csharp
  private static void DisplayInfo(string title, int id)
  {
      int threadId = Thread.CurrentThread.ManagedThreadId;
      string synchronizationContext = SynchronizationContext.Current?.GetType().FullName ?? "<null>";
  
      Console.WriteLine($"{title} ({id})");
      Console.WriteLine($"    - Thread ({id}): {threadId}");
      Console.WriteLine($"    - SynchronizationContext ({id}): {synchronizationContext}");
  }
  ```


### Step 1

Call the asynchronous code 4 times in a loop:

- Extract the awaited call together with the two `Display` calls into a separate method
- Call the asynchronous method in a loop of 4 iterations.
- Run the application and highlight that the synchronization context is null.

### Step 2

Create a custom synchronization context:

- Create and add a custom synchronization context before the awaited call.
- Put a breakpoint on the `Send` and `Post` methods.
- Run the application and highlight that the 
  - `await` is always calling the `Post` method
  - the initial synchronization context is lost after the first awaited call. The `Post` method is called only first time.

### Step 3

Update the custom synchronization context to propagate itself on the new thread.

- Fix it by setting the synchronization context from the `Post` method in order to propagate it.
- Run the application and highlight that the synchronization context is propagated to the next calls.


### Step 4

Replace `await` with `ContinueWith`:

- Include all the code after the `await` in a local function called `Continuation`.
- Replace the `await` keyword with a call to `ContinueWith` that calls the `Continuation` function.
- Run the application. Note that for each next iteration, before the "awaited" asynchronous task, the synchronization context is always present, but it is lost inside the `Continuation` method.

### Step 4

Capture and use the synchronization context:

- Capture the `SynchronizationContext` before the "awaited" asynchronous call.
- Inside the `ContinueWith`, use the previously captured synchronization context to call the continuation.
- Run the application again and highlight that the synchronization context is preserved, but we have another problem:
    - The continuation is not really awaited. This is happening because the `SynchronizationContext.Post` method that we call is asynchronous.

### Step 5

Wait until the `SynchronizationContext.Post` method is finished:

- Use a `ManualResetEventSlim` to wait until the continuation is finished.

- **Note**: We reproduced the behaviour, but I am not sure how the `async-await` mechanism really awaits for the continuation to finish.

