using UnityEngine;

public class FortuneWheelManager : MonoBehaviour
{
    private bool _isStarted;
    private float[] _sectorsAngles;
    private float _finalAngle;
    private float _startAngle = 0;
    private float _currentLerpRotationTime;
    public GameObject Circle; 			// Rotatable Object with rewards
    //public Text CoinsDeltaText; 		// Pop-up text with wasted or rewarded coins amount
    //public Text CurrentCoinsText; 		// Pop-up text with wasted or rewarded coins amount
    //public int TurnCost = 300;			// How much coins user waste when turn whe wheel
    //public int CurrentCoinsAmount = 1000;	// Started coins amount. In your project it can be set up from CoinsManager or from PlayerPrefs and so on
    //public int PreviousCoinsAmount;		// For wasted coins animation

    private bool isTurning;

    [SerializeField] private AbilityCard[] abilityCards;

    private void OnEnable()
    {
        EventManager.StartListening(EventManager.GameEventEnum.ON_LUCKY_WHEEL_APPREAR, UpdateWheelDataDisplay);
    }

    private void OnDisable()
    {
        EventManager.StopListening(EventManager.GameEventEnum.ON_LUCKY_WHEEL_APPREAR, UpdateWheelDataDisplay);
    }

    public void UpdateWheelDataDisplay()
    {
        foreach (var card in abilityCards)
        {
            card.CardData = AbilitiesManager.Instance.GetRandomAbility();
            card.UpdateCardInfo();
        }
    }

    public void TurnWheel()
    {
        if (isTurning)
        {
            return;
        }

        isTurning = true;
        // Player has enough money to turn the wheel
        //if (CurrentCoinsAmount >= TurnCost)
        //{
        _currentLerpRotationTime = 0f;

        // Fill the necessary angles (for example if you want to have 12 sectors you need to fill the angles with 30 degrees step)
        _sectorsAngles = new float[] { 45, 90, 135, 180, 225, 270, 315, 360 };

        int fullCircles = 5;
        float randomFinalAngle = _sectorsAngles[UnityEngine.Random.Range(0, _sectorsAngles.Length)];

        // Here we set up how many circles our wheel should rotate before stop
        _finalAngle = -(fullCircles * 360 + randomFinalAngle);
        _isStarted = true;

        //    PreviousCoinsAmount = CurrentCoinsAmount;

        //    // Decrease money for the turn
        //    CurrentCoinsAmount -= TurnCost;

        //    // Show wasted coins
        //    CoinsDeltaText.text = "-" + TurnCost;
        //    CoinsDeltaText.gameObject.SetActive(true);

        //    // Animate coins
        //    StartCoroutine(HideCoinsDelta());
        //    StartCoroutine(UpdateCoinsAmount());
        //}
    }

    private void GiveAwardByAngle()
    {
        // Here you can set up rewards for every sector of wheel
        switch ((int)_startAngle)
        {
            case 0:
                RewardsAbility(0);
                break;
            case -315:
                RewardsAbility(abilityCards.Length - 1);
                break;
            case -270:
                RewardsAbility(abilityCards.Length - 2);
                break;
            case -225:
                RewardsAbility(abilityCards.Length - 3);
                break;
            case -180:
                RewardsAbility(abilityCards.Length - 4);
                break;
            case -135:
                RewardsAbility(abilityCards.Length - 5);
                break;
            case -90:
                RewardsAbility(abilityCards.Length - 6);
                break;
            case -45:
                RewardsAbility(abilityCards.Length - 7);
                break;
            default:
                RewardsAbility(0);
                break;
        }

        GameManager.Instance.CloseLuckyWheelCanvasThenSetFlagCantRollLuckWheel();

        //wait until animation done
        Invoke(nameof(ResetTurn), 1f);
    }

    private void ResetTurn()
    {
        //set for next turn
        isTurning = false;
    }

    void Update()
    {
        //// Make turn button non interactable if user has not enough money for the turn
        //if (_isStarted || CurrentCoinsAmount < TurnCost)
        //{
        //    TurnButton.interactable = false;
        //    TurnButton.GetComponent<Image>().color = new Color(255, 255, 255, 0.5f);
        //}
        //else
        //{
        //    TurnButton.interactable = true;
        //    TurnButton.GetComponent<Image>().color = new Color(255, 255, 255, 1);
        //}

        if (!_isStarted)
            return;

        float maxLerpRotationTime = 4f;

        // increment timer once per frame
        _currentLerpRotationTime += Time.deltaTime;
        if (_currentLerpRotationTime > maxLerpRotationTime || Circle.transform.eulerAngles.z == _finalAngle)
        {
            _currentLerpRotationTime = maxLerpRotationTime;
            _isStarted = false;
            _startAngle = _finalAngle % 360;

            GiveAwardByAngle();
            //StartCoroutine(HideCoinsDelta());
        }

        // Calculate current position using linear interpolation
        float t = _currentLerpRotationTime / maxLerpRotationTime;

        // This formulae allows to speed up at start and speed down at the end of rotation.
        // Try to change this values to customize the speed
        t = t * t * t * (t * (6f * t - 15f) + 10f);

        float angle = Mathf.Lerp(_startAngle, _finalAngle, t);
        Circle.transform.eulerAngles = new Vector3(0, 0, angle);
    }

    private void RewardsAbility(int index)
    {
        SoundManager.PlaySFX("Recharge Energy or Skill UI Sound 7");
        abilityCards[index].ChooseMe(true);
    }

    //private void RewardCoins(int awardCoins)
    //{
    //    CurrentCoinsAmount += awardCoins;
    //    CoinsDeltaText.text = "+" + awardCoins;
    //    CoinsDeltaText.gameObject.SetActive(true);
    //    StartCoroutine(UpdateCoinsAmount());
    //}

    //private IEnumerator HideCoinsDelta()
    //{
    //    yield return new WaitForSeconds(1f);
    //    CoinsDeltaText.gameObject.SetActive(false);
    //}

    //private IEnumerator UpdateCoinsAmount()
    //{
    //    // Animation for increasing and decreasing of coins amount
    //    const float seconds = 0.5f;
    //    float elapsedTime = 0;

    //    while (elapsedTime < seconds)
    //    {
    //        CurrentCoinsText.text = Mathf.Floor(Mathf.Lerp(PreviousCoinsAmount, CurrentCoinsAmount, (elapsedTime / seconds))).ToString();
    //        elapsedTime += Time.deltaTime;

    //        yield return new WaitForEndOfFrame();
    //    }

    //    PreviousCoinsAmount = CurrentCoinsAmount;
    //    CurrentCoinsText.text = CurrentCoinsAmount.ToString();
    //}
}
