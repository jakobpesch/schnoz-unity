using System;
namespace Schnoz
{
  [Serializable]
  public static class RuleCollection
  {
    public static Rule NullRule = new Rule("NullRule", (_) => new RuleEvaluation(0, null));
  }
}
