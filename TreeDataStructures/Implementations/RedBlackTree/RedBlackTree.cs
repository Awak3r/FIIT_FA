using TreeDataStructures.Core;

namespace TreeDataStructures.Implementations.RedBlackTree;

public class RedBlackTree<TKey, TValue> : BinarySearchTreeBase<TKey, TValue, RbNode<TKey, TValue>>
    where TKey : IComparable<TKey>
{
    private static RbColor ColorOf(RbNode<TKey, TValue>? node) => node?.Color ?? RbColor.Black;
    private static bool IsRed(RbNode<TKey, TValue>? node) => ColorOf(node) == RbColor.Red;
    private static bool IsBlack(RbNode<TKey, TValue>? node) => ColorOf(node) == RbColor.Black;

    private static void SetColor(RbNode<TKey, TValue>? node, RbColor color)
    {
        if (node != null)
        {
            node.Color = color;
        }
    }

    private static RbNode<TKey, TValue> Minimum(RbNode<TKey, TValue> node)
    {
        RbNode<TKey, TValue> current = node;
        while (current.Left != null)
        {
            current = current.Left;
        }
        return current;
    }

    protected override RbNode<TKey, TValue> CreateNode(TKey key, TValue value)
    {
        return new RbNode<TKey, TValue>(key, value) { Color = RbColor.Red };
    }
    
    protected override void OnNodeAdded(RbNode<TKey, TValue> newNode)
    {
        RbNode<TKey, TValue> node = newNode;

        while (node.Parent != null && IsRed(node.Parent))
        {
            RbNode<TKey, TValue> parent = node.Parent;
            RbNode<TKey, TValue>? grand = parent.Parent;
            if (grand == null)
            {
                break;
            }

            if (parent == grand.Left)
            {
                RbNode<TKey, TValue>? uncle = grand.Right;
                if (IsRed(uncle))
                {
                    SetColor(parent, RbColor.Black);
                    SetColor(uncle, RbColor.Black);
                    SetColor(grand, RbColor.Red);
                    node = grand;
                }
                else
                {
                    if (node == parent.Right)
                    {
                        node = parent;
                        RotateLeft(node);
                        parent = node.Parent!;
                        grand = parent.Parent!;
                    }

                    SetColor(parent, RbColor.Black);
                    SetColor(grand, RbColor.Red);
                    RotateRight(grand);
                }
            }
            else
            {
                RbNode<TKey, TValue>? uncle = grand.Left;
                if (IsRed(uncle))
                {
                    SetColor(parent, RbColor.Black);
                    SetColor(uncle, RbColor.Black);
                    SetColor(grand, RbColor.Red);
                    node = grand;
                }
                else
                {
                    if (node == parent.Left)
                    {
                        node = parent;
                        RotateRight(node);
                        parent = node.Parent!;
                        grand = parent.Parent!;
                    }

                    SetColor(parent, RbColor.Black);
                    SetColor(grand, RbColor.Red);
                    RotateLeft(grand);
                }
            }
        }

        if (Root != null)
        {
            Root.Color = RbColor.Black;
        }
    }

    protected override void OnNodeRemoved(RbNode<TKey, TValue>? parent, RbNode<TKey, TValue>? child)
    {
        DeleteFixup(child, parent);
        if (Root != null)
        {
            Root.Color = RbColor.Black;
        }
    }

    protected override void RemoveNode(RbNode<TKey, TValue> node)
    {
        RbNode<TKey, TValue> replacement = node;
        RbColor replacementOriginalColor = replacement.Color;
        RbNode<TKey, TValue>? fixNode;
        RbNode<TKey, TValue>? fixParent;

        if (node.Left == null)
        {
            fixNode = node.Right;
            fixParent = node.Parent;
            Transplant(node, node.Right);
        }
        else if (node.Right == null)
        {
            fixNode = node.Left;
            fixParent = node.Parent;
            Transplant(node, node.Left);
        }
        else
        {
            replacement = Minimum(node.Right);
            replacementOriginalColor = replacement.Color;
            fixNode = replacement.Right;

            if (replacement.Parent == node)
            {
                fixParent = replacement;
            }
            else
            {
                fixParent = replacement.Parent;
                Transplant(replacement, replacement.Right);
                replacement.Right = node.Right;
                replacement.Right!.Parent = replacement;
            }

            Transplant(node, replacement);
            replacement.Left = node.Left;
            replacement.Left!.Parent = replacement;
            replacement.Color = node.Color;
        }

        if (replacementOriginalColor == RbColor.Black)
        {
            OnNodeRemoved(fixParent, fixNode);
        }
    }

    private void DeleteFixup(RbNode<TKey, TValue>? node, RbNode<TKey, TValue>? parent)
    {
        while (node != Root && IsBlack(node) && parent != null)
        {
            if (node == parent.Left)
            {
                FixWhenNodeIsLeftChild(ref node, ref parent);
            }
            else
            {
                FixWhenNodeIsRightChild(ref node, ref parent);
            }
        }

        SetColor(node, RbColor.Black);
    }

    private void FixWhenNodeIsLeftChild(ref RbNode<TKey, TValue>? node, ref RbNode<TKey, TValue>? parent)
    {
        RbNode<TKey, TValue>? sibling = parent!.Right;

        if (IsRed(sibling))
        {
            SetColor(sibling, RbColor.Black);
            SetColor(parent, RbColor.Red);
            RotateLeft(parent);
            sibling = parent.Right;
        }

        if (IsBlack(sibling?.Left) && IsBlack(sibling?.Right))
        {
            SetColor(sibling, RbColor.Red);
            node = parent;
            parent = node.Parent;
            return;
        }

        if (IsBlack(sibling?.Right))
        {
            SetColor(sibling?.Left, RbColor.Black);
            SetColor(sibling, RbColor.Red);
            if (sibling != null) { RotateRight(sibling); }
            sibling = parent.Right;
        }

        SetColor(sibling, ColorOf(parent));
        SetColor(parent, RbColor.Black);
        SetColor(sibling?.Right, RbColor.Black);
        RotateLeft(parent);
        node = Root;
        parent = null;
    }

    private void FixWhenNodeIsRightChild(ref RbNode<TKey, TValue>? node, ref RbNode<TKey, TValue>? parent)
    {
        RbNode<TKey, TValue>? sibling = parent!.Left;

        if (IsRed(sibling))
        {
            SetColor(sibling, RbColor.Black);
            SetColor(parent, RbColor.Red);
            RotateRight(parent);
            sibling = parent.Left;
        }

        if (IsBlack(sibling?.Right) && IsBlack(sibling?.Left))
        {
            SetColor(sibling, RbColor.Red);
            node = parent;
            parent = node.Parent;
            return;
        }

        if (IsBlack(sibling?.Left))
        {
            SetColor(sibling?.Right, RbColor.Black);
            SetColor(sibling, RbColor.Red);
            if (sibling != null) { RotateLeft(sibling); }
            sibling = parent.Left;
        }

        SetColor(sibling, ColorOf(parent));
        SetColor(parent, RbColor.Black);
        SetColor(sibling?.Left, RbColor.Black);
        RotateRight(parent);
        node = Root;
        parent = null;
    }
}
