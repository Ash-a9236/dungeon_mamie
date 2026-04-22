using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(menuName = "2D/Tiles/Sibling Rule Tile")]
public class SiblingRuleTile : RuleTile
{
    [Tooltip("Add tiles here that should connect with this one.")]
    public TileBase[] siblings;

    public override bool RuleMatch(int neighbor, TileBase tile)
    {
        switch (neighbor)
        {
            // If the rule looks for a green arrow (This), it accepts itself OR a sibling.
            case TilingRuleOutput.Neighbor.This:
                return tile == this || IsSibling(tile);
            
            // If the rule looks for a red X (Not This), it rejects itself AND its siblings.
            case TilingRuleOutput.Neighbor.NotThis:
                return tile != this && !IsSibling(tile);
        }
        return base.RuleMatch(neighbor, tile);
    }

    private bool IsSibling(TileBase tile)
    {
        if (siblings == null || tile == null) return false;
        foreach (var sibling in siblings)
        {
            if (sibling == tile) return true;
        }
        return false;
    }
}