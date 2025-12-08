# About
Sharp.Collections is a .NET 8.0 library that contains implementations of custom collections. It builds on top of the [Sharp](https://github.com/Feralnex/Sharp) library, leveraging its low-level utilities and extensions to provide efficient, memory-aware, and flexible data structures.

- `Buffer<TItem>`
- `Segment<TItem>`
- `ConcurrentSegment<TItem>`
- `Segments<TItem>`
- `Queue<TItem>`
- `ConcurrentQueue<TItem>`

## Buffer\<TItem\> : IBuffer\<TItem\>
A simple fixed-size collection that writes elements at the Tail position and reads elements at the Head position.

```csharp
public interface IBuffer<TItem>
{
    int Head { get; }
    int Tail { get; }
    int Count { get; }
    int Size { get; }

    void Write(TItem item, bool moveTail = true);
    bool TryWrite(TItem item, bool moveTail = true);
    TItem Read(bool moveHead = true);
    bool TryRead(out TItem? item, bool moveHead = true);
}
```

## Segment\<TItem\> : Buffer\<TItem\>, ISegment\<TItem\>
An extended buffer with the NextHead and NextTail properties.

```csharp
public interface ISegment<TItem> : IBuffer<TItem>
{
    ISegment<TItem>? NextHead { get; set; }
    ISegment<TItem>? NextTail { get; set; }
}
```

## ConcurrentSegment\<TItem\> : Segment\<TItem\>
A segment adapted to a concurrent environment.

## Segments\<TItem\> : ISegments<\TItem\>
A collection of two unidirectional branches of stored segments used in a custom queue implementation.

```csharp
public interface ISegments<TItem>
{
    ISegment<TItem> Head { get; }
    ISegment<TItem> Tail { get; }

    void AddToHead(ISegment<TItem> segment);
    void AddToTail(ISegment<TItem> segment);
    void MoveToNextHead();
    void MoveToNextTail();
}
```

## Queue\<TItem\> : IQueue\<TItem\>
A custom queue implementation focusing on more efficient use of allocated memory. It consists of segments that are reused or new ones are allocated in case of memory shortage. To summarize, it is a queue implementation that reuses allocated memory without deallocating it - it only grows and never shrinks.

```csharp
public interface IQueue<TItem>
{
    int Count { get; }

    void Enqueue(TItem item);
    bool TryEnqueue(TItem item);
    TItem Dequeue();
    bool TryDequeue(out TItem? item);
    TItem Peek();
    bool TryPeek(out TItem? item);
}
```

## ConcurrentQueue\<TItem\> : Queue\<TItem\>
A queue adapted to a concurrent environment.

## DictionaryExtensions
A set of extension methods for `Dictionary<TKey, TValue>` that provide additional functionality, including safe retrieval, insertion, and access to internal structures depending on process architecture (x64 or x86). These extensions rely on `ArchitectureExtensions` from the [Sharp](https://github.com/Feralnex/Sharp) library.

### Methods
- **GetOrAdd<TKey, TValue>(key)**  
  Retrieves the value for the given key, or adds a new instance of `TValue` if the key is missing.

- **GetOrAdd<TKey, TValue>(key, onMissingKey)**  
  Retrieves the value for the given key, or adds a new value created by the provided delegate if the key is missing.

- **GetBuckets / SetBuckets**  
  Access or modify the internal bucket array of the dictionary.

- **GetEntries / SetEntries**  
  Access or modify the internal entries array.

- **TryGetFastModMultiplier / TrySetFastModMultiplier**  
  Access or modify the fast modulus multiplier used internally (only available on x64).

- **GetCount / SetCount**  
  Access or modify the dictionary’s count field.

- **GetFreeCount / SetFreeCount**  
  Access or modify the free count field.

- **GetFreeList / SetFreeList**  
  Access or modify the free list index.

- **GetVersion / SetVersion**  
  Access or modify the dictionary’s version field.

- **GetComparer / SetComparer**  
  Access or modify the equality comparer used by the dictionary.

- **GetKeys / SetKeys**  
  Access or modify the dictionary’s key collection.

- **GetValues / SetValues**  
  Access or modify the dictionary’s value collection.

### Entry<TKey, TValue>
Represents the internal structure of a dictionary entry.

```csharp
public struct Entry<TKey, TValue>
    where TKey : notnull
{
    public uint hashCode;
    public int next;   // index of next entry in chain or free list
    public TKey key;   // key of entry
    public TValue value; // value of entry
}
```

## ListExtensions
A set of extension methods for `List<T>` that provide access to internal fields and additional utility operations. These extensions use unsafe casting to interact with the internal structure of `List<T>`.

### Methods

- **GetItems<TTarget>(list)**  
  Retrieves the internal array of items backing the list.

- **SetItems<TTarget>(list, value)**  
  Replaces the internal array of items with the provided array.

- **GetCount<TTarget>(list)**  
  Retrieves the current count of elements in the list.

- **SetCount<TTarget>(list, value)**  
  Sets the count of elements in the list.

- **GetVersion<TTarget>(list)**  
  Retrieves the internal version number of the list (used for enumerator validation).

- **SetVersion<TTarget>(list, value)**  
  Sets the internal version number of the list.

- **GetWithTheLeastCountOrAdd<TCollection>(list, maxSize, onCollectionsFull)**  
  Finds the collection within the list that has the least number of elements.  
  - If all collections have a count greater than or equal to `maxSize`, a new collection is created using the provided delegate and added to the list.  
  - Returns the collection with the least count or the newly added collection.

### UnsafeList<TTarget>
Represents the internal structure of `List<T>` used by the extensions.

```csharp
private class UnsafeList<TTarget>
{
    private TTarget[] _items;
    private int _size;
    private int _version;

    public TTarget[] Items
    {
        get => _items;
        set => _items = value;
    }
    public int Count
    {
        get => _size;
        set => _size = value;
    }
    public int Version
    {
        get => _version;
        set => _version = value;
    }
}
```

## ArrayExtensions
A set of extension methods for arrays that provide advanced operations, including unsafe pointer casting and optimized memory swapping using hardware acceleration (SIMD).

### Methods
- **AsPointers<TType>(array)**  
  Casts a managed array of reference types (`TType[]`) into an array of raw pointers (`IntPtr[]`).  
  Useful for scenarios requiring direct pointer manipulation.

- **TrySwap(source, sourceIndex, destination, destinationIndex, length)**  
  Safely swaps a block of bytes between two arrays.  
  - Returns `false` if indices or length are out of bounds.  
  - Returns `true` and performs the swap if parameters are valid.

- **DangerousSwap(source, sourceIndex, destination, destinationIndex, length)**  
  Performs an unsafe swap of bytes between two arrays using hardware acceleration when available.  
  - Uses **Vector512**, **Vector256**, and **Vector128** intrinsics if supported by the CPU.  
  - Falls back to swapping using `ulong`, `uint`, `ushort`, and `byte` operations when SIMD is not available.  
  - Optimized for performance by processing data in chunks of decreasing size until all bytes are swapped.

## NativeList<TElement>
A low-level collection type that manages unmanaged memory directly using `NativeMemory`.  
It provides a way to allocate, free, and reference native memory blocks while maintaining a reverse cache for safe conversions between pointers and `NativeList<TElement>` instances.

### Definition
```csharp
public class NativeList<TElement>
    where TElement : unmanaged
{
    protected static ConcurrentDictionary<IntPtr, NativeList<TElement>> ReverseCache { get; }

    private IntPtr _pointer;

    public nuint Size { get; }

    public unsafe NativeList(nuint size);
    ~NativeList();

    public static implicit operator NativeList<TElement>(IntPtr pointer);
    public static implicit operator IntPtr(NativeList<TElement> nativeList);
}
```

## SpanExtensions
A set of extension methods for `Span<T>` that provide specialized search functionality for numeric types implementing `INumber<TValue>` and unsafe copy operations for character spans.

### Methods
- **IndexOfAnyNumberExcept<TValue>(span, value)**  
  Searches the span for the first index containing a number that is **not equal** to the specified `value`.  
  - Starts at the beginning of the span.  
  - Returns the index of the first non-matching element, or `-1` if all elements match.  

- **IndexOfAnyNumberExcept<TValue>(span, value, offset)**  
  Searches the span starting from the given `offset` for the first index containing a number that is **not equal** to the specified `value`.  
  - Returns the index of the first non-matching element after the offset, or `-1` if none.  

- **CopyTo(ReadOnlySpan<char> source, sbyte* destination)**  
  Copies characters from a `ReadOnlySpan<char>` into an unmanaged `sbyte*` buffer.  
  - Each character is converted to its ASCII representation if it is within the range `0x00–0x7F`.  
  - Characters outside this range are replaced with `'?'` (`0x3F`).  
  - A null terminator (`0`) is appended at the end of the copied sequence.  
  - Useful for interop scenarios where unmanaged code expects null-terminated ASCII strings.  

## Selection<TTarget>
An abstract base class representing a selection of targets, built on top of `Maybe<TTarget>` (from the [Sharp](https://github.com/Feralnex/Sharp) library).  
It provides mechanisms for handling multiple targets (`Many`) or none, with support for functional-style operations (`IfSome`, `IfNone`, `Match`).

### Many (Nested Class)
Represents a selection containing multiple targets.  
Internally uses an `IterableList` to store and manage targets, with support for event-driven iteration via `Iterator`.

#### Features
- **Targets**: A `List<TTarget>` containing the selected items.  
- **HasSome**: Always `true` for `Many`.  
- **Count**: Number of selected targets.  
- **TryGetAt(index, out target)**: Attempts to retrieve a target at the specified index.  
- **GetAll()**: Returns all targets as a `ReadOnlySpan<TTarget>`.  
- **Clear()**: Removes all targets.  
- **IfSome / IfNone**: Functional-style branching based on presence of targets.  
- **Match**: Pattern matching with actions or functions.  
- **Iterator**: A stack-allocated helper for iterating with subscription to `IterableList` events.

### IterableList (Nested Class)
A custom list implementation wrapping `List<TTarget>` with added event support.

#### Features
- **Added / Removed events**: Triggered when items are added or removed.  
- **Source**: Underlying `List<TTarget>`.  
- **AsSpan / AsReadOnlySpan**: Provides direct span access to the list’s items.  
- **Add, Insert, Remove, RemoveAt, Clear**: Standard list operations with event notifications.  
- **Enumerator support**: Implements `IList<TTarget>` and provides enumerators.

### Selection<TTarget> (Base Class)
Provides the abstraction for handling selections with validation and transformation logic.

#### Features
- **HasSome / Count**: Delegated to the current selection (`Many` or `None`).  
- **TryGet / TryGetAt**: Retrieve targets or a specific target.  
- **Add / Remove / Clear**: Manage targets dynamically.  
- **Functional methods**: `IfSome`, `IfNone`, `Match` for branching and pattern matching.  
- **Initialize**: Builds a `Many` selection from arrays or lists, applying validation and filtering.  
- **Abstract methods**:  
  - `Validate(target)`  
  - `TryGetFirstTargetIndex(source, length, out index)`  
  - `CountTargets(source, startIndex, length)`  
  - `FillTargets(source, startIndex, destination, destinationIndex, length)`

## References<TTarget>
A concrete implementation of `Selection<TTarget>` for **reference types** (`class`).  
Filters out `null` references and ensures only valid targets are included.

#### Features
- **Validation**: Ensures target references are not `IntPtr.Zero`.  
- **Implicit conversions**:  
  - `TTarget → References<TTarget>`  
  - `TTarget[] → References<TTarget>`  
- **Target filtering**: Uses pointer-based checks (`AsPointers`) to skip null references.  
- **CountTargets / FillTargets**: Efficiently processes arrays, excluding null entries.

## Values<TTarget>
A concrete implementation of `Selection<TTarget>` for **value types** (`struct`).  
Includes all values without filtering.

#### Features
- **Validation**: Always returns `true`.  
- **Implicit conversions**:  
  - `TTarget → Values<TTarget>`  
  - `TTarget[] → Values<TTarget>`  
- **CountTargets**: Counts all elements from the given start index.  
- **FillTargets**: Copies values directly into the destination 

## Pooling System
The library provides a flexible pooling system for managing reusable objects.  
It includes both generic and keyed pools, with thread-safe variants for concurrent environments.

### IPool / IPool<TElement>
Defines the contract for a pool of reusable objects.

```csharp
public interface IPool
{
    bool IsThreadSafe { get; }
    int Count { get; }

    object? Acquire();
    object Acquire(Func<object> createNewOverride);
    bool TryAcquire(out object? element);
    bool TryAcquire(out object? element, Func<object> createNewOverride);
    void Release(object element);
    bool TryRelease(object element);
}

public interface IPool<TElement> : IPool
    where TElement : class
{
    new TElement? Acquire();
    TElement Acquire(Func<TElement> createNewOverride);
    bool TryAcquire(out TElement? element);
    bool TryAcquire(out TElement? element, Func<TElement> createNewOverride);
    void Release(TElement element);
    bool TryRelease(TElement element);
}
```

## Pool<TElement>
A basic pool implementation for reference types.  
Uses a queue to store and reuse objects, with optional custom creation logic.

### Features
- **Acquire / TryAcquire**: Retrieves an object from the pool or creates a new one.  
- **Release / TryRelease**: Returns an object to the pool.  
- **IsThreadSafe**: `false` by default.  
- **Initialization**: Can be constructed with segment size or custom creation delegates.  
- **Fallback creation**: Uses default constructor if available.

## ConcurrentPool<TElement>
A thread-safe variant of `Pool<TElement>` using `ConcurrentQueue<TElement>` internally.

### Features
- **IsThreadSafe**: Always `true`.  
- **Initialization**: Supports segment size and custom creation delegates.  
- **Concurrent operations**: Safe for multi-threaded environments.

## Pools (Static Class)
Manages collections of pools by element type.  
Provides global registration, retrieval, and removal of pools.

### Features
- **Add / TryAdd / Remove / RemoveAll**: Manage pools globally.  
- **Contains / Any**: Check for pool existence.  
- **GetOrAdd**: Retrieve or create a pool if missing.  
- **TryGet / UnsafeTryGet**: Attempt to retrieve pools with optional selectors.  
- **Selectors**: Support custom selection logic across multiple pools.

## IKeyedPool / IKeyedPool<TKey, TElement>
Defines the contract for pools keyed by a specific type.

```csharp
public interface IKeyedPool
{
    bool IsThreadSafe { get; }

    int Count(object key);
    object Acquire(object key);
    object Acquire(object key, Func<object, object> createNewOverride);
    bool TryAcquire(object key, out object? element);
    bool TryAcquire(object key, Func<object, object> createNewOverride, out object? element);
    void Release(object key, object element);
    bool TryRelease(object key, object element);
}

public interface IKeyedPool<TKey, TElement> : IKeyedPool
    where TKey : notnull
    where TElement : class
{
    int Count(TKey key);
    TElement Acquire(TKey key);
    TElement Acquire(TKey key, Func<TKey, TElement> createNewOverride);
    bool TryAcquire(TKey key, out TElement? element);
    bool TryAcquire(TKey key, Func<TKey, TElement> createNewOverride, out TElement? element);
    void Release(TKey key, TElement element);
    bool TryRelease(TKey key, TElement element);
}
```

## KeyedPool<TKey, TElement>
A pool implementation keyed by `TKey`.  
Each key maps to its own queue of pooled elements.

### Features
- **Buckets**: Dictionary mapping keys to queues.  
- **SegmentSize**: Optional configuration for queue segment size.  
- **Acquire / TryAcquire**: Retrieves or creates objects per key.  
- **Release / TryRelease**: Returns objects to the appropriate key bucket.  
- **IsThreadSafe**: `false` by default.  
- **Fallback creation**: Uses default constructor if available.

## ConcurrentKeyedPool<TKey, TElement>
Thread-safe variant of `KeyedPool<TKey, TElement>` using `ConcurrentDictionary` and `ConcurrentQueue`.

### Features
- **IsThreadSafe**: Always `true`.  
- **Buckets**: Concurrent dictionary for key-to-queue mapping.  
- **Concurrent operations**: Safe for multi-threaded environments.

## KeyedPools (Static Class)
Manages collections of keyed pools globally.  
Pools are stored by composite handles (`KeyType`, `ElementType`).

### Features
- **Add / Remove / RemoveAll**: Manage keyed pools globally.  
- **Contains / Any**: Check for keyed pool existence.  
- **GetOrAdd**: Retrieve or create keyed pools if missing.  
- **TryGet / UnsafeTryGet**: Attempt to retrieve keyed pools with optional selectors.  
- **Selectors**: Support custom selection logic across multiple keyed pools.

## Defaults
The library provides default configuration values through the Defaults static class. These values can be customized via DefaultSettings using the external Settings provider from [Sharp](https://github.com/Feralnex/Sharp) library.

```csharp
public static class Defaults
{
    public static int SegmentSize { get; }

    static Defaults()
    {
        SegmentSize = Settings.DangerousTryGet(out DefaultSettings? settings)
            ? settings!.SegmentSize
            : IntPtr.Size * Constants.ByteSizeInBits;
    }
}
```

## DefaultSettings
A configuration class used to define default values for the library.

```csharp
public class DefaultSettings
{
    public int SegmentSize { get; init; }
}
```