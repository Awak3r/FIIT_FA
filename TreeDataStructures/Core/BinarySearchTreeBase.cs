using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using TreeDataStructures.Interfaces;

namespace TreeDataStructures.Core;

public abstract class BinarySearchTreeBase<TKey, TValue, TNode>(IComparer<TKey>? comparer = null)
    : ITree<TKey, TValue>
    where TNode : Node<TKey, TValue, TNode>
{
    protected TNode? Root;
    public IComparer<TKey> Comparer { get; protected set; } = comparer ?? Comparer<TKey>.Default; // use it to compare Keys

    public int Count { get; protected set; }

    public bool IsReadOnly => false;

    public ICollection<TKey> Keys => InOrder().Select(x => x.Key).ToList();
    public ICollection<TValue> Values => InOrder().Select(x => x.Value).ToList();


    public virtual void Add(TKey key, TValue value)
    {
        if (Root == null)
        {
            Root = CreateNode(key, value);
            Count = 1;
            OnNodeAdded(Root);
            return;
        }
        TNode? current = Root;
        TNode? parent = null;
        while (current != null)
        {
            parent = current;
            int compRes = Comparer.Compare(key, current.Key);
            if (compRes == 0)
            {
                current.Value = value;
                return;
            }
            else if (compRes > 0)
            {
                current = current.Right;
            }
            else
            {
                current = current.Left;
            }
        }
        TNode newNode = CreateNode(key, value);
        newNode.Parent = parent;
        int res = Comparer.Compare(key, parent!.Key);
        if (res > 0)
        {
            parent.Right = newNode;
        }
        else
        {
            parent.Left = newNode;
        }
        Count++;
        OnNodeAdded(newNode);
    }


    public virtual bool Remove(TKey key)
    {
        TNode? node = FindNode(key);
        if (node == null) { return false; }

        RemoveNode(node);
        this.Count--;
        return true;
    }


    protected virtual void RemoveNode(TNode node)
    {
        TNode? parentAfterRemove;
        TNode? childAfterRemove;
        if (node.Left == null)
        {
            parentAfterRemove = node.Parent;
            childAfterRemove = node.Right;
            Transplant(node, node.Right);
            OnNodeRemoved(parentAfterRemove, childAfterRemove);
            return;
        }
        else if (node.Right == null)
        {
            parentAfterRemove = node.Parent;
            childAfterRemove = node.Left;
            Transplant(node, node.Left);
            OnNodeRemoved(parentAfterRemove, childAfterRemove);
            return;
        }
        TNode nextNode = node.Right;
        while (nextNode.Left != null)
        {
            nextNode = nextNode.Left;
        }
        if (nextNode.Parent != node)
        {
            parentAfterRemove = nextNode.Parent;
            childAfterRemove = nextNode.Right;
            Transplant(nextNode, nextNode.Right);
            nextNode.Right = node.Right;
            nextNode.Right!.Parent = nextNode;
        }
        else
        {
            parentAfterRemove = nextNode;
            childAfterRemove = nextNode.Right;
        }
        Transplant(node, nextNode);
        nextNode.Left = node.Left;
        nextNode.Left!.Parent = nextNode;
        OnNodeRemoved(parentAfterRemove, childAfterRemove);
    }

    public virtual bool ContainsKey(TKey key) => FindNode(key) != null;

    public virtual bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value)
    {
        TNode? node = FindNode(key);
        if (node != null)
        {
            value = node.Value;
            return true;
        }
        value = default;
        return false;
    }

    public TValue this[TKey key]
    {
        get => TryGetValue(key, out TValue? val) ? val : throw new KeyNotFoundException();
        set => Add(key, value);
    }


    #region Hooks

    /// <summary>
    /// Вызывается после успешной вставки
    /// </summary>
    /// <param name="newNode">Узел, который встал на место</param>
    protected virtual void OnNodeAdded(TNode newNode) { }

    /// <summary>
    /// Вызывается после удаления. 
    /// </summary>
    /// <param name="parent">Узел, чей ребенок изменился</param>
    /// <param name="child">Узел, который встал на место удаленного</param>
    protected virtual void OnNodeRemoved(TNode? parent, TNode? child) { }

    #endregion


    #region Helpers
    protected abstract TNode CreateNode(TKey key, TValue value);


    protected TNode? FindNode(TKey key)
    {
        TNode? current = Root;
        while (current != null)
        {
            int cmp = Comparer.Compare(key, current.Key);
            if (cmp == 0) { return current; }
            current = cmp < 0 ? current.Left : current.Right;
        }
        return null;
    }

    protected void RotateLeft(TNode x)
    {
        TNode? y = x.Right;
        if (y == null) { return; }

        x.Right = y.Left;
        if (y.Left != null){
            y.Left.Parent = x;
        }

        y.Parent = x.Parent;
        if (x.Parent == null){
            Root = y;
        }
        else if (x.IsLeftChild){
            x.Parent.Left = y;
        }
        else{
            x.Parent.Right = y;
        }

        y.Left = x;
        x.Parent = y;
    }

    protected void RotateRight(TNode y)
    {
        TNode? x = y.Left;
        if (x == null) { return; }

        y.Left = x.Right;
        if (x.Right != null){
            x.Right.Parent = y;
        }

        x.Parent = y.Parent;
        if (y.Parent == null){
            Root = x;
        }
        else if (y.IsLeftChild){
            y.Parent.Left = x;
        }
        else{
            y.Parent.Right = x;
        }

        x.Right = y;
        y.Parent = x;
    }

    protected void RotateBigLeft(TNode x)
    {
        if (x.Right == null) { return; }
        RotateRight(x.Right);
        RotateLeft(x);
    }

    protected void RotateBigRight(TNode y)
    {
        if (y.Left == null) { return; }
        RotateLeft(y.Left);
        RotateRight(y);
    }

    protected void RotateDoubleLeft(TNode x)
    {
        RotateLeft(x);
        if (x.Parent != null){
            RotateLeft(x.Parent);
        }
    }

    protected void RotateDoubleRight(TNode y)
    {
        RotateRight(y);
        if (y.Parent != null){
            RotateRight(y.Parent);
        }
    }

    protected void Transplant(TNode u, TNode? v)
    {
        if (u.Parent == null)
        {
            Root = v;
        }
        else if (u.IsLeftChild)
        {
            u.Parent.Left = v;
        }
        else
        {
            u.Parent.Right = v;
        }
        v?.Parent = u.Parent;
    }
    #endregion

    public IEnumerable<TreeEntry<TKey, TValue>> InOrder() => InOrderTraversal(Root, 0);

    private IEnumerable<TreeEntry<TKey, TValue>> InOrderTraversal(TNode? node, int depth)
    {
        if (node == null) { yield break; }
        foreach(var entry in InOrderTraversal(node.Left, depth + 1)) {yield return entry; }
        yield return new TreeEntry<TKey, TValue>(node.Key, node.Value, depth);
        foreach(var entry in InOrderTraversal(node.Right, depth + 1)) {yield return entry; }
    }

    public IEnumerable<TreeEntry<TKey, TValue>> PreOrder() => PreOrderTraversal(Root, 0);

    private IEnumerable<TreeEntry<TKey, TValue>> PreOrderTraversal(TNode? node, int depth)
    {
        if (node == null) { yield break; }
        yield return new TreeEntry<TKey, TValue>(node.Key, node.Value, depth);
        foreach (var entry in PreOrderTraversal(node.Left, depth + 1)) { yield return entry; }
        foreach (var entry in PreOrderTraversal(node.Right, depth + 1)) { yield return entry; }
    }
    public IEnumerable<TreeEntry<TKey, TValue>> PostOrder() => PostOrderTraversal(Root, 0);

    private IEnumerable<TreeEntry<TKey, TValue>> PostOrderTraversal(TNode? node, int depth)
    {
        if (node == null) { yield break; }
        foreach (var entry in PostOrderTraversal(node.Left, depth + 1)) { yield return entry; }
        foreach (var entry in PostOrderTraversal(node.Right, depth + 1)) { yield return entry; }
        yield return new TreeEntry<TKey, TValue>(node.Key, node.Value, depth);
    }

    public IEnumerable<TreeEntry<TKey, TValue>> InOrderReverse() => InOrderReverseTraversal(Root, 0);

    private IEnumerable<TreeEntry<TKey, TValue>> InOrderReverseTraversal(TNode? node, int depth)
    {
        if (node == null) { yield break; }
        foreach (var entry in InOrderReverseTraversal(node.Right, depth + 1)) { yield return entry; }
        yield return new TreeEntry<TKey, TValue>(node.Key, node.Value, depth);
        foreach (var entry in InOrderReverseTraversal(node.Left, depth + 1)) { yield return entry; }
    }

    public IEnumerable<TreeEntry<TKey, TValue>> PreOrderReverse() => PreOrder().Reverse();
    public IEnumerable<TreeEntry<TKey, TValue>> PostOrderReverse() => PostOrder().Reverse();

    /// <summary>
    /// Внутренний класс-итератор. 
    /// Реализует паттерн Iterator вручную, без yield return (ban).
    /// </summary>
    private struct TreeIterator :
        IEnumerable<TreeEntry<TKey, TValue>>,
        IEnumerator<TreeEntry<TKey, TValue>>
    {
        private readonly TNode? _root;
        private readonly TraversalStrategy _strategy; // or make it template parameter?
        private List<TreeEntry<TKey, TValue>>? _entries;
        private int _index;
        private TreeEntry<TKey, TValue> _current;

        public TreeIterator(TNode? root, TraversalStrategy strategy)
        {
            _root = root;
            _strategy = strategy;
            _entries = null;
            _index = -1;
            _current = default;
        }

        public IEnumerator<TreeEntry<TKey, TValue>> GetEnumerator() => new TreeIterator(_root, _strategy);
        IEnumerator IEnumerable.GetEnumerator() => new TreeIterator(_root, _strategy);

        public TreeEntry<TKey, TValue> Current => _current;
        object IEnumerator.Current => _current;


        public bool MoveNext()
        {
            EnsureEntries();
            if (_entries == null) { return false; }
            if (_index + 1 >= _entries.Count) { return false; }
            _index++;
            _current = _entries[_index];
            return true;
        }

        public void Reset()
        {
            _index = -1;
            _current = default;
        }


        public void Dispose()
        {
            // TODO release managed resources here
        }

        private void EnsureEntries()
        {
            if (_entries != null) { return; }

            _entries = [];
            if (_strategy == TraversalStrategy.InOrder){
                CollectInOrder(_root, 0, _entries);
            }
            else if (_strategy == TraversalStrategy.PreOrder){
                CollectPreOrder(_root, 0, _entries);
            }
            else if (_strategy == TraversalStrategy.PostOrder){
                CollectPostOrder(_root, 0, _entries);
            }
            else if (_strategy == TraversalStrategy.InOrderReverse){
                CollectInOrderReverse(_root, 0, _entries);
            }
            else if (_strategy == TraversalStrategy.PreOrderReverse){
                CollectPreOrderReverse(_root, 0, _entries);
            }
            else if (_strategy == TraversalStrategy.PostOrderReverse){
                CollectPostOrderReverse(_root, 0, _entries);
            }
        }

        private static void CollectInOrder(TNode? node, int depth, List<TreeEntry<TKey, TValue>> result)
        {
            if (node == null) { return; }
            CollectInOrder(node.Left, depth + 1, result);
            result.Add(new TreeEntry<TKey, TValue>(node.Key, node.Value, depth));
            CollectInOrder(node.Right, depth + 1, result);
        }

        private static void CollectPreOrder(TNode? node, int depth, List<TreeEntry<TKey, TValue>> result)
        {
            if (node == null) { return; }
            result.Add(new TreeEntry<TKey, TValue>(node.Key, node.Value, depth));
            CollectPreOrder(node.Left, depth + 1, result);
            CollectPreOrder(node.Right, depth + 1, result);
        }

        private static void CollectPostOrder(TNode? node, int depth, List<TreeEntry<TKey, TValue>> result)
        {
            if (node == null) { return; }
            CollectPostOrder(node.Left, depth + 1, result);
            CollectPostOrder(node.Right, depth + 1, result);
            result.Add(new TreeEntry<TKey, TValue>(node.Key, node.Value, depth));
        }

        private static void CollectInOrderReverse(TNode? node, int depth, List<TreeEntry<TKey, TValue>> result)
        {
            if (node == null) { return; }
            CollectInOrderReverse(node.Right, depth + 1, result);
            result.Add(new TreeEntry<TKey, TValue>(node.Key, node.Value, depth));
            CollectInOrderReverse(node.Left, depth + 1, result);
        }

        private static void CollectPreOrderReverse(TNode? node, int depth, List<TreeEntry<TKey, TValue>> result)
        {
            if (node == null) { return; }
            result.Add(new TreeEntry<TKey, TValue>(node.Key, node.Value, depth));
            CollectPreOrderReverse(node.Right, depth + 1, result);
            CollectPreOrderReverse(node.Left, depth + 1, result);
        }

        private static void CollectPostOrderReverse(TNode? node, int depth, List<TreeEntry<TKey, TValue>> result)
        {
            if (node == null) { return; }
            CollectPostOrderReverse(node.Right, depth + 1, result);
            CollectPostOrderReverse(node.Left, depth + 1, result);
            result.Add(new TreeEntry<TKey, TValue>(node.Key, node.Value, depth));
        }
    }


    private enum TraversalStrategy { InOrder, PreOrder, PostOrder, InOrderReverse, PreOrderReverse, PostOrderReverse }

    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
    {
        return InOrder()
            .Select(x => new KeyValuePair<TKey, TValue>(x.Key, x.Value))
            .GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();


    public void Add(KeyValuePair<TKey, TValue> item) => Add(item.Key, item.Value);
    public void Clear() { Root = null; Count = 0; }
    public bool Contains(KeyValuePair<TKey, TValue> item) => ContainsKey(item.Key);
    public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
    {
        if (array == null) { throw new ArgumentException("is null"); }
        if (arrayIndex < 0 || arrayIndex > array.Length) { throw new ArgumentException("Idx err"); }
        if (array.Length - arrayIndex < Count) { throw new ArgumentException("wrong length"); }
        int i = arrayIndex;
        foreach (var kv in this)
        {
            array[i++] = kv;
        }
    }
    public bool Remove(KeyValuePair<TKey, TValue> item) => Remove(item.Key);
}
