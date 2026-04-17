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
    }

    public override bool Remove(TKey key)
    {
        BstNode<TKey, TValue>? current = Root;
        BstNode<TKey, TValue>? last = null;

        while (current != null)
        {
            last = current;
            int cmp = Comparer.Compare(key, current.Key);
            if (cmp == 0)
            {
                break;
            }

            current = cmp < 0 ? current.Left : current.Right;
        }

        if (current == null)
        {
            if (last != null)
            {
                Splay(last);
            }

            return false;
        }

        Splay(current);

        BstNode<TKey, TValue>? leftSubtree = Root!.Left;
        BstNode<TKey, TValue>? rightSubtree = Root.Right;

        if (leftSubtree != null)
        {
            leftSubtree.Parent = null;
        }

        if (rightSubtree != null)
        {
            rightSubtree.Parent = null;
        }

        Root.Left = null;
        Root.Right = null;

        if (leftSubtree == null)
        {
            Root = rightSubtree;
        }
        else
        {
            Root = leftSubtree;

            BstNode<TKey, TValue> maxLeft = leftSubtree;
            while (maxLeft.Right != null)
            {
                maxLeft = maxLeft.Right;
            }

            Splay(maxLeft);
            Root!.Right = rightSubtree;
            if (rightSubtree != null)
            {
                rightSubtree.Parent = Root;
            }
        }

        Count--;
        return true;
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
