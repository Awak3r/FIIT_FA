using System.Diagnostics.CodeAnalysis;
using TreeDataStructures.Implementations.BST;

namespace TreeDataStructures.Implementations.Splay;

public class SplayTree<TKey, TValue> : BinarySearchTree<TKey, TValue>
{
    protected override BstNode<TKey, TValue> CreateNode(TKey key, TValue value)
        => new(key, value);

    private void Splay(BstNode<TKey, TValue> node)
    {
        while (node.Parent != null)
        {
            BstNode<TKey, TValue> parent = node.Parent;
            BstNode<TKey, TValue>? grand = parent.Parent;

            if (grand == null)
            {
                if (node.IsLeftChild) { RotateRight(parent); }
                else { RotateLeft(parent); }
            }
            else if ((node.IsLeftChild && parent.IsLeftChild) || (node.IsRightChild && parent.IsRightChild))
            {
                if (node.IsLeftChild)
                {
                    RotateRight(grand);
                    RotateRight(parent);
                }
                else
                {
                    RotateLeft(grand);
                    RotateLeft(parent);
                }
            }
            else
            {
                if (node.IsLeftChild)
                {
                    RotateRight(parent);
                    RotateLeft(grand);
                }
                else
                {
                    RotateLeft(parent);
                    RotateRight(grand);
                }
            }
        }
    }
    
    protected override void OnNodeAdded(BstNode<TKey, TValue> newNode)
    {
        Splay(newNode);
    }
    
    protected override void OnNodeRemoved(BstNode<TKey, TValue>? parent, BstNode<TKey, TValue>? child)
    {
        if (parent != null)
        {
            Splay(parent);
        }
        else if (child != null)
        {
            Splay(child);
        }
    }

    public override bool ContainsKey(TKey key)
    {
        return TryGetValue(key, out _);
    }
    
    public override bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value)
    {
        BstNode<TKey, TValue>? current = Root;
        BstNode<TKey, TValue>? last = null;
        while (current != null)
        {
            last = current;
            int cmp = Comparer.Compare(key, current.Key);
            if (cmp == 0)
            {
                value = current.Value;
                Splay(current);
                return true;
            }
            current = cmp < 0 ? current.Left : current.Right;
        }

        if (last != null) { Splay(last); }
        value = default;
        return false;
    }
    
}
