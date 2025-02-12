using UnityEngine;
using UnityEngine.UI;

public class HowToPlayController : MonoBehaviour
{
  public GameObject howToPlayPanel; // How to Play 창
  public Image contentPanel; // 페이지를 보여줄 이미지
  public Button prevButton, nextButton; // 좌우 화살표 버튼
  public Sprite[] pages; // 페이지 이미지들

  private int currentPage = 0;

  void Start()
  {
    UpdatePage(); // 처음 시작할 때 페이지 설정
  }

  public void OpenHowToPlay()
  {
    howToPlayPanel.SetActive(true); // 창 열기
    currentPage = 0; // 첫 페이지로 설정
    UpdatePage();
  }

  public void CloseHowToPlay()
  {
    howToPlayPanel.SetActive(false); // 창 닫기
  }

  public void NextPage()
  {
    if (currentPage < pages.Length - 1)
    {
      currentPage++;
      UpdatePage();
    }
  }

  public void PrevPage()
  {
    if (currentPage > 0)
    {
      currentPage--;
      UpdatePage();
    }
  }

  void UpdatePage()
  {
    // 현재 페이지에 맞는 이미지 설정
    contentPanel.sprite = pages[currentPage];

    // 첫 페이지일 경우 "이전 버튼" 숨기기
    prevButton.gameObject.SetActive(currentPage > 0);

    // 마지막 페이지일 경우 "다음 버튼" 숨기기
    nextButton.gameObject.SetActive(currentPage < pages.Length - 1);
  }
}
