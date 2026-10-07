# 순천 메타버스 도서관

순천시립도서관을 메타버스 공간으로 옮긴 크로스플랫폼(모바일·웹) 멀티플레이 서비스.
도서관 공간 탐방, 개인서재 꾸미기와 방문, 방명록·추천도서 커뮤니티, 포토존, NPC 안내 등을 제공했다. (현재 서비스 종료)

| 항목 | 내용 |
|---|---|
| 기간 | 개발 6개월 |
| 인원 | 3인 (클라이언트 2, 서버 1) |
| 담당 | 개인서재 시스템, 오브젝트 상호작용, 커뮤니티(방명록·추천도서·투표), 포토존, NPC |
| 플랫폼 | 모바일 (Android/iOS), 웹 (WebGL) |
| 기술 | Unity, C#, Photon PUN 2, REST API (HTTP), Newtonsoft.Json |

> **안내**
> - 서비스 종료된 프로젝트에서 본인 담당 코드만 발췌한 저장소로, 단독으로 빌드되지 않는다.
> - 공개를 위해 주석 정리와 리팩터링(중복 클래스 통합, 버그 수정)을 거친 버전으로, 서비스 당시 코드와 구조가 다를 수 있다.
> - 네트워크 기반(Photon 설정, NetworkManager), 공통 UI 매니저, 웹뷰, 서버 통신 유틸리티와 데이터 클래스는 팀원 작업이라 포함하지 않았다.

---

## 폴더 구성

### `Scripts/MyRoom` — 개인서재 인테리어
| 파일 | 역할 |
|---|---|
| `FurnitureManager.cs` | 가구 생성·삭제, 배치와 벽지·바닥 색상을 JSON으로 직렬화해 서버에 저장·불러오기, 신규 유저 초기화 |
| `FurnitureMove.cs` | 가구 드래그 이동(바닥·벽 구분), 겹침 시 빨간 아웃라인과 원위치 복귀, 회전, 배치 확정 |
| `PositionCheck.cs` | 가구 겹침 판정 |
| `Inven_Controller.cs` | 가구 수납함: 카테고리 필터, 배치된 가구 비활성화, 전체 수납, 업적 연동 모던 가구 잠금 해제 |
| `RoomColorChanger.cs` | 벽지·바닥 색상 변경 (프리셋 + RGB 슬라이더) |
| `SetTrophy.cs` | 서버 업적 정보로 트로피 진열 |
| `RoomCameraSetting.cs` | 서재 전용 카메라 전환 |
| `HasBookCheck.cs` | 도서관 대출현황 조회 (모바일 웹뷰 / 웹 새 창) |
| 기타 | 가구 식별 정보, 수납함 버튼 |

### `Scripts/Interaction` — 오브젝트 상호작용
| 파일 | 역할 |
|---|---|
| `IClickable.cs` | 클릭 가능한 오브젝트 인터페이스 |
| `ClickableHoverHandler.cs` | 아웃라인 강조, 이름 표시, 오클릭 방지(누른 대상 = 뗀 대상) 공통 로직 |
| `PlayerObjInteraction.cs` | 광장·도서관에서의 상호작용 (거리 제한, UI 사용 중 차단) |
| `ButtonHoverDetector.cs` / `AddHoverDetectorToButtons.cs` | UI 버튼 뒤의 3D 오브젝트가 함께 클릭되는 문제 방지 |
| `InteractionManager.cs` | 팝업을 여닫을 때 상호작용 끄기·켜기 |
| `ObjNameText.cs` | 커서를 따라다니는 오브젝트 이름 |

### `Scripts/Visit` — 서재 방문 (멀티플레이)
| 파일 | 역할 |
|---|---|
| `InvadeLib.cs` | 다른 유저의 서재 방문 |
| `RoomPlayerCheck.cs` | 방문 인원 제한 |
| `UI_LibMembers.cs` / `LibMemberObject.cs` | 방문자 목록, 우클릭 메뉴 |
| `UI_Interaction_Room.cs` / `UI_Kick.cs` / `AllExport.cs` | 방문자 개별·전체 내보내기 (Photon RPC) |

### `Scripts/Community` — 커뮤니티
| 파일 | 역할 |
|---|---|
| `GuestBook_*.cs` | 방명록: 작성(금칙어 응답 처리), 정렬, 주인 삭제·방문자 신고, 블라인드, 읽음 처리, 새 글 알림 |
| `ArchivesPC_UI.cs` / `BookRecommend.cs` / `Recommend_UI.cs` 등 | 추천도서 게시판: 목록·작성·열람, 본인 글 삭제, 타인 글 신고 |
| `Vote.cs` / `Vote_UI.cs` | 투표: 진행 여부 확인, 항목 선택, 확인 후 전송 |

### `Scripts/PhotoZone` — 포토존
| 파일 | 역할 |
|---|---|
| `PhotoZone.cs` | 촬영 모드(다른 플레이어 숨김, 전용 카메라), 배경·포즈, 플랫폼별 저장 (에디터 파일 / 웹 다운로드 / 모바일 갤러리) |
| `PlayerNameController.cs` | 이름표가 상황별 카메라(일반·포토존·서재)를 바라보도록 처리 |

### `Scripts/Web`, `Scripts/NPC`, `Scripts/UI`
| 파일 | 역할 |
|---|---|
| `WebViewInteractable.cs` | 웹 연결 오브젝트 공통 클래스 (플랫폼별 웹뷰 / 새 창) |
| `ClubMeetingWebView.cs` | 동아리 회의 중일 때만 회의 자료 열기 |
| `WebViewButton.cs` / `ArchivesBookBtn.cs` / `WebViewCloseNotifier.cs` | 자료실 웹 메뉴, 웹뷰 정리 |
| `Mascot.cs` | 마스코트 NPC (근접 시 말풍선·바라보기, 대화창 또는 웹 연결) |
| `Tutorial.cs` | 플랫폼별 튜토리얼, 첫 입장 시 지도 안내 |

---

## 포함하지 않은 의존 코드
`NetworkManager`, `GameManager`, `UIInteractionManager`, `UIInteractionManager_Room`, `UI_PlayerInteraction`, `UI_WebView`, `ButtonControl`, `UTILS`(HTTP 요청), `Suncheon.WebData`(요청·응답 데이터 클래스), `PlayerManager`, `PlayerMoveManager`, `PlayerAnimManager`, `CameraManager`, `UI_YesNoPopUp`, `VoteObject`, `VoteSelectBtn`, `Outline`, 스크린샷 에셋, Photon PUN 2 등
