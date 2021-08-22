using System.Collections.Generic;
using Schnoz;
using UnityEngine;
namespace Schnoz
{
  public class SchnozViewModel : MonoBehaviour
  {
    [SerializeField] private Schnoz schnoz;
    private void Start()
    {
      this.schnoz.Start();

      List<RuleLogic> ruleLogics = new List<RuleLogic>();
      ruleLogics.Add(RuleLogicMethods.DiagonalToTopRight);
    }
  }
}
