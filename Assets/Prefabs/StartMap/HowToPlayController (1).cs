using UnityEngine;
using UnityEngine.UI;

public class HowToPlayManager : MonoBehaviour
{
  public GameObject howToPlayPanel;   // How to Play 패널
  public GameObject[] pages;          // 여러 개의 페이지 오브젝트들
  public Text guideText;              // 하단 안내 문구

  private int currentPage = 0;        // 현재 페이지 번호

  void Start()
  {
    howToPlayPanel.SetActive(false);  // 시작할 때 패널 숨김
  }

  // 📌 How to Play 패널 열기
  public void OpenHowToPlay()
  {
    howToPlayPanel.SetActive(true); // 패널 활성화
    currentPage = 0;  // 첫 페이지로 초기화

    // 모든 페이지 비활성화 후 첫 번째 페이지 활성화
    foreach ( GameObject page in pages )
    {
      page.SetActive(false);
    }

    pages[currentPage].SetActive(true); // 첫 번째 페이지 활성화
    UpdatePage(); // 페이지 업데이트 실행 (자식 오브젝트들 활성화)
  }

  // 📌 터치하면 다음 페이지로 이동
  void Update()
  {
    if ( howToPlayPanel.activeSelf && Input.anyKeyDown )  // 아무 키나 터치하면
    {
      NextPage();
    }
  }

  public void NextPage()
  {
    if ( currentPage < pages.Length - 1 )
    {
      pages[currentPage].SetActive(false); // 현재 페이지 숨기기
      currentPage++;  // 다음 페이지로 이동
      pages[currentPage].SetActive(true); // 다음 페이지 표시
      UpdatePage();
    }
    else
    {
      CloseHowToPlay();  // 마지막 페이지면 닫기
    }
  }

  void UpdatePage()
  {
    // 마지막 페이지인지 확인하고 안내 텍스트 변경
    if ( currentPage == pages.Length - 1 )
    {
      guideText.text = "아무 곳을 터치하면 화면이 사라집니다";
    }
    else
    {
      guideText.text = "아무 곳을 터치하여 다음장을 넘기세요";
    }

    // 🔥 현재 페이지의 모든 자식 오브젝트들 다시 활성화하기 🔥
    foreach ( Transform child in pages[currentPage].transform )
    {
      child.gameObject.SetActive(true);
    }
  }

  public void CloseHowToPlay()
  {
    pages[currentPage].SetActive(false); // 현재 페이지 숨기기
    howToPlayPanel.SetActive(false);  // 패널 닫기
  }
}
