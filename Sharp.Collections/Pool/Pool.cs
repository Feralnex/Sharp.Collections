using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Sharp.Collections
{
    public class Pool<TElement> : IPool<TElement>
        where TElement : class
    {
        private static readonly Reference<Func<TElement>> _creator;

        private readonly Reference<Func<TElement>> _createNew;

        protected IQueue<TElement> Elements { get; private set; }

        public virtual bool IsThreadSafe => false;
        public int Count => Elements.Count;

        public Pool()
        {
            _createNew = new Reference<Func<TElement>>();
            Elements = InitializeQueue();
        }

        public Pool(int segmentSize)
        {
            _createNew = new Reference<Func<TElement>>();
            Elements = InitializeQueue(segmentSize);
        }

        public Pool(Func<TElement> onCreateNew) : this()
            => _createNew.Set(onCreateNew);

        public Pool(int segmentSize, Func<TElement> onCreateNew) : this(segmentSize)
            => _createNew.Set(onCreateNew);

        static Pool()
        {
            ConstructorInfo? constructorInfo = typeof(TElement).GetConstructor(Type.EmptyTypes);

            _creator = new Reference<Func<TElement>>();

            if (constructorInfo is not null)
            {
                Func<TElement> creator = Expression
                    .Lambda<Func<TElement>>(Expression.New(constructorInfo))
                    .Compile();

                _creator.Set(creator);
            }
        }

        public TElement Acquire()
        {
            if (Elements.TryDequeue(out TElement? element))
                return element!;
            else if (_createNew.TryGet(out Func<TElement>? createNew))
                return createNew!();
            else if (_creator.TryGet(out Func<TElement>? creator))
                return creator!();

            throw new InvalidOperationException();
        }

        public TElement Acquire(Func<TElement> createNewOverride)
        {
            if (!Elements.TryDequeue(out TElement? element))
                element = createNewOverride();

            return element!;
        }

        public bool TryAcquire(out TElement? element)
        {
            bool acquired = false;

            if (Elements.TryDequeue(out element))
            {
                acquired = true;
            }
            else if (_createNew.TryGet(out Func<TElement>? createNew))
            {
                element = createNew!();
                acquired = true;
            }
            else if (_creator.TryGet(out Func<TElement>? creator))
            {
                element = creator!();
                acquired = true;
            }

            return acquired;
        }

        public bool TryAcquire(out TElement? element, Func<TElement> createNewOverride)
        {
            if (!Elements.TryDequeue(out TElement? acquiredElement))
                acquiredElement = createNewOverride();

            element = acquiredElement;

            return true;
        }

        public void Release(TElement element)
            => Elements.Enqueue(element);

        public bool TryRelease(TElement element)
            => Elements.TryEnqueue(element);

        protected virtual IQueue<TElement> InitializeQueue()
            => new Queue<TElement>();

        protected virtual IQueue<TElement> InitializeQueue(int segmentSize)
            => new Queue<TElement>(segmentSize);

        #region IPool

        object IPool.Acquire()
            => Acquire();

        object IPool.Acquire(Func<object> createNewOverride)
        {
            if (createNewOverride is not Func<TElement> createNew)
                throw new InvalidOperationException();

            return Acquire(createNew);
        }

        bool IPool.TryAcquire(out object? element)
        {
            element = default;

            if (!TryAcquire(out TElement? acquiredElement))
                return false;

            element = acquiredElement;

            return true;
        }

        bool IPool.TryAcquire(out object? element, Func<object> createNewOverride)
        {
            element = default;

            if (!Elements.TryDequeue(out TElement? acquiredElement))
            {
                if (createNewOverride is not Func<TElement> createNew)
                    return false;

                acquiredElement = createNew();
            }

            element = acquiredElement;

            return true;
        }

        void IPool.Release(object element)
        {
            if (element is not TElement)
                throw new InvalidOperationException();

            TElement elementToRelease = Unsafe.As<TElement>(element);

            Release(elementToRelease);
        }

        bool IPool.TryRelease(object element)
        {
            if (element is not TElement)
                return false;

            TElement elementToRelease = Unsafe.As<TElement>(element);

            return TryRelease(elementToRelease);
        }

        #endregion IPool
    }
}
