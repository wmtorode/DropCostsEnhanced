using BattleTech.UI.TMProWrapper;
using BattleTech.UI;
using BattleTech;
using UnityEngine;
using UnityEngine.Events;

namespace DropCostsEnhanced
{

  //Cant seem to make this load, probably some unity thing I dont fully understand
  [UIModule.PrefabNameAttr("uixPrfPanl_SellConfirmation-Popup")]
  public class DifficultySelectPopUp : UIModule
  {
    [SerializeField] private GameObject builtInFadeObj;
    [SerializeField] private LocalizableText CurrentQuantityText;
    [SerializeField] private LocalizableText InventoryQuantityText;
    [SerializeField] private LocalizableText TitleText;
    [SerializeField] private LocalizableText DescriptionText;
    [SerializeField] private HBSDOTweenButton IncreaseButton;
    [SerializeField] private HBSDOTweenButton DecreaseButton;
    [SerializeField] private HBSDOTweenButton MaxButton;
    [SerializeField] private HBSDOTweenButton MinButton;
    [SerializeField] private HBSDOTweenButton ConfirmButton;
    [SerializeField] private HBSDOTweenButton CancelButton;
    protected SimGameState simState;
    private UnityAction<int> OnConfirmCB;
    private int globalDifficulty;

    public void SetData(
      SimGameState sim,
      UnityAction<int> OnConfirmCB = null,
      UnityAction OnCancelCB = null)
    {
      this.simState = sim;
      this.globalDifficulty = sim.CompanyStats.GetValue<int>(DifficultyManager.SettableDifficultyStat);
      this.builtInFadeObj.SetActive(true);
      this.OnConfirmCB = OnConfirmCB;
      this.Refresh();
    }

    public void Refresh()
    {
      this.TitleText.SetText("SET GLOBAL DIFFICULTY:");
      this.InventoryQuantityText.SetText("");
      this.CurrentQuantityText.SetText("");
      this.DescriptionText.SetText("");
      if (this.globalDifficulty >= DCECore.settings.maxDifficulty)
      {
        this.IncreaseButton.SetState(ButtonState.Disabled);
        this.MaxButton.SetState(ButtonState.Disabled);
      }
      else
      {
        this.IncreaseButton.SetState(ButtonState.Enabled);
        this.MaxButton.SetState(ButtonState.Enabled);
      }

      if (this.globalDifficulty <= 1)
      {
        this.DecreaseButton.SetState(ButtonState.Disabled);
        this.MinButton.SetState(ButtonState.Disabled);
      }
      else
      {
        this.DecreaseButton.SetState(ButtonState.Enabled);
        this.MinButton.SetState(ButtonState.Enabled);
      }
    }

    public override void ReceiveButtonPress(string button)
    {
      if (!(button == "Up"))
      {
        if (!(button == "Down"))
        {
          if (!(button == "Max"))
          {
            if (!(button == "Min"))
            {
              if (!(button == "Cancel"))
              {
                if (button == "Confirm")
                  this.OnConfirm();
              }
              else
                this.OnCancel();
            }
            else
              this.globalDifficulty = 1;
          }
          else
            this.globalDifficulty = DCECore.settings.maxDifficulty;
        }
        else if (this.globalDifficulty > 1)
          --this.globalDifficulty;
      }
      else if (this.globalDifficulty < DCECore.settings.maxDifficulty)
        ++this.globalDifficulty;

      this.Refresh();
    }

    public void OnConfirm()
    {
      if (this.OnConfirmCB != null)
        this.OnConfirmCB(this.globalDifficulty);
      this.simState.CompanyStats.Set(DifficultyManager.SettableDifficultyStat, globalDifficulty);
      this.builtInFadeObj.SetActive(false);
      this.Pool();
    }

    public void OnCancel()
    {
      this.builtInFadeObj.SetActive(false);
      this.Pool();
    }

    public override bool HandleEscapeKeypress()
    {
      this.OnCancel();
      return true;
    }

    public override bool HandleEnterKeypress()
    {
      if (this.ConfirmButton.gameObject.activeInHierarchy)
        this.OnConfirm();
      else
        this.OnCancel();
      return true;
    }
  }
}