# 유니티 강화학습
<img src="https://user-images.githubusercontent.com/41245985/57083071-f05e1200-6d32-11e9-9a57-692d790f0119.PNG"></img>  

<img src="https://user-images.githubusercontent.com/41245985/57514723-0af44480-734c-11e9-9471-056c4a9e756a.png"></img>  
## 설치
1. Visual Studio 설치 (유니티 개발환경 설치 체크, 유니티2018은 체크 해제)  
2. Unity 홈페이지에서 Unity hub 설치   
```
Visual Studio 설치 체크 해제할 것
설치 화면 맨 아래에 한글 언어팩 체크

설치 완료 후 아무 프로젝트나 열고 상단 메뉴에서
Edit -> Preferences -> Languages -> Editor Language(위에서 2번째)를 한국어로 바꾸고 Unity 종료
```
3. Anaconda 설치, 설치과정 중 환경변수 추가하여 conda 및 python 명령어가 실행되도록 할 것.
```
%UserProfile%\Anaconda3\Scripts
%UserProfile%\Anaconda3\Scripts\conda.exe
%UserProfile%\Anaconda3
%UserProfile%\Anaconda3\python.exe
```
4. Anaconda Prompt 관리자 권한 실행, 파이썬 3.6.8로 가상환경 생성  
```
conda create -n ml-agents python=3.6.8
```
5. 가상환경 활성화  
```
activate ml-agents
```
6. Tensorflow 설치 (GPU 쓰고 싶으면 Tensorflow GPU 설치방법을 검색할 것)  
```
pip install tensorflow==1.7.1
```
7. 적당한 디렉토리에 mlagents 다운로드 (내 문서 추천)  
```
git clone https://github.com/Unity-Technologies/ml-agents.git
```
8. 가상환경에 mlagents를 설치한다. (activate ml-agents 상태여야 함)  
```
pip install mlagents
```
설치 완료  

## 설정
1. Robomaster Unity 환경을 실행한다.  
```
Project Open -> Robomaster_Unity_RL
```
2. 상단 메뉴에서 프로젝트 설정을 수정  
```
상단 메뉴 -> 프로젝트 설정 -> Player -> 기타 설정 클릭(확장됨) -> 설정 아래쪽에 적힌 것들을 바꿔야 함

스크립팅 런타임 버전 - .NET 4.x에 상응
API 호환성 수준 - .NET 4.x
'안전하지 않은' 코드 허용 - 체크

mlagent 깃을 보면 스크립팅 정의 심볼에 ENABLE_TENSORFLOW 를 입력하라고 되어있는데
요즘 버전에서는 이거 입력하면 작동 안함
절대 입력하면 안됨
```
<img src="https://user-images.githubusercontent.com/41245985/57082413-9f015300-6d31-11e9-829d-0456ea873f44.png"></img>  


## 구현한 것
모든 경기 요소 완벽히 구현 완료  
메카넘 휠, 리로드, 쉴드, HP, 탄 발사  
State를 알려줄 UI는 추후 구현 예정  

네비게이션은 쓸모가 없음 길찾기 알고리즘으로 직접 제작해야 함  
RoboState에서 모든 로봇 정보를 가져옴  
강화학습을 하려면 RoboAgent코드만 수정하면 됨  
강화학습을 하는 방법은 아래 튜토리얼을 해 볼 것  
https://github.com/Unity-Technologies/ml-agents/blob/master/docs/Learning-Environment-Create-New.md  
