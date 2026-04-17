using TreeDataStructures.Core;

namespace TreeDataStructures.Implementations.Treap;

public class Treap<TKey, TValue> : BinarySearchTreeBase<TKey, TValue, TreapNode<TKey, TValue>>
{
    protected override TreapNode<TKey, TValue> CreateNode(TKey key, TValue value)
    {
        return new TreapNode<TKey, TValue>(key, value);
    }

    protected override void OnNodeAdded(TreapNode<TKey, TValue> newNode)
    {
        while (newNode.Parent != null && newNode.Parent.Priority < newNode.Priority)
        {
            if (newNode.IsLeftChild)
            {
                RotateRight(newNode.Parent);
            }
            else
            {
                RotateLeft(newNode.Parent);
            }
        }
    }

    protected override void RemoveNode(TreapNode<TKey, TValue> node)
    {
        while (node.Left != null && node.Right != null)
        {
            if (node.Left.Priority >= node.Right.Priority)
            {
                RotateRight(node);
            }
            else
            {
                RotateLeft(node);
            }
        }

        base.RemoveNode(node);
    }

    protected override void OnNodeRemoved(TreapNode<TKey, TValue>? parent, TreapNode<TKey, TValue>? child)
    {
    }
}
