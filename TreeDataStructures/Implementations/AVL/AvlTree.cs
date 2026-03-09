using TreeDataStructures.Core;

namespace TreeDataStructures.Implementations.AVL;

public class AvlTree<TKey, TValue> : BinarySearchTreeBase<TKey, TValue, AvlNode<TKey, TValue>>
    where TKey : IComparable<TKey>
{
    protected override AvlNode<TKey, TValue> CreateNode(TKey key, TValue value)
        => new(key, value);

    private static int GetHeight(AvlNode<TKey, TValue>? node) => node?.Height ?? 0;

    private static void UpdateHeight(AvlNode<TKey, TValue> node)
    {
        int leftHeight = GetHeight(node.Left);
        int rightHeight = GetHeight(node.Right);
        int maxHeight = leftHeight > rightHeight ? leftHeight : rightHeight;
        node.Height = 1 + maxHeight;
    }

    private static int GetBalance(AvlNode<TKey, TValue> node)
    {
        return GetHeight(node.Left) - GetHeight(node.Right);
    }

    private void Zabalansit(AvlNode<TKey, TValue>? node)
    {
        while (node != null)
        {
            UpdateHeight(node);
            int balance = GetBalance(node);
            if (balance > 1)
            {
                if (node.Left != null && GetBalance(node.Left) < 0)
                {
                    RotateLeft(node.Left);
                    UpdateHeight(node.Left);
                }
                RotateRight(node);
                UpdateHeight(node);
                if (node.Parent != null) { UpdateHeight(node.Parent); }
            }
            else if (balance < -1)
            {
                if (node.Right != null && GetBalance(node.Right) > 0)
                {
                    RotateRight(node.Right);
                    UpdateHeight(node.Right);
                }
                RotateLeft(node);
                UpdateHeight(node);
                if (node.Parent != null) { UpdateHeight(node.Parent); }
            }
            node = node.Parent;
        }
    }
    
    protected override void OnNodeAdded(AvlNode<TKey, TValue> newNode)
    {
        Zabalansit(newNode);
    }

}