# Escape Hospital
Unity 3D 맵 구조변경 시스템을 도입한 공포게임
<img width="1009" alt="eh_mainImage" src="https://github.com/DimBlessing/DimBlessing/assets/47944912/0ec342f9-b0d0-4de8-992e-9e0635bf41d1">
개발기간: 2023.03 ~ 2023.08

## 팀 소개
조윤장 - 전남대학교 소프트웨어공학과
<br>
정준희 - 전남대학교 소프트웨어공학과
<br>
이준형 - 전남대학교 소프트웨어공학과
<br>

## 프로젝트 소개
본 프로젝트는 선형적인 방탈출 게임 장르에 비선형적 진행 요소를 더하여 플레이의 자유도를 높인 공포 게임 제작을 목표로 한다.
<br><br>
따라서 게임 진행 중에 플레이어가 임의로 맵의 구조를 변경할 수 있는 기능을 도입하여 직접적인 플레이의 자유도를 높였으며
<br>
과도한 자유도로 인한 공포감 저하를 방지하기 위해 적대적 NPC의 무작위 이동 기능을 구현하여 예측 불가능한 공포감을 유도하여
<br>
비선형적 진행 방식의 자유도와 함께 공포게임의 주 목적인 공포감 제공을 동시에 달성할 수 있도록 프로젝트를 진행한다. 
<br>

## 시작 가이드
**Development Requirements**
<br>
Unity Editor 2021.3.20f1 LTS ~
<br>
<br>
**System Requirements**
<br>
|           |          Minimum          |        Recommended        |
|-----------|:-------------------------:|:-------------------------:|
|     OS    |  Windwos 7<br>Mac OS Big Sur | Windows 10<br>Mac OS Ventura |
| Processor | Intel Core Duo 2<br>Apple M1 |   Intel i3-8100<br>Apple M1  |
|   Memory  |            8GB            |            8GB            |
|  Graphics |     Integrated Graphic    |     Integrated Graphic    |
|  Storage  |            1GB            |            2GB            |
<br>

## Stacks
**Environment**
<br>
<img src="https://img.shields.io/badge/Visual Studio Code-007ACC?style=flat-square&logo=visualstudiocode&logoColor=white"/>
<img src="https://img.shields.io/badge/GitHub-181717?style=flat-square&logo=github&logoColor=white"/>
<br>
**Development**
<br>
<img src="https://img.shields.io/badge/Unity-000000?style=flat-square&logo=unity&logoColor=white"/>
<img src="https://img.shields.io/badge/CSharp-239128?style=flat-square&logo=Csharp&logoColor=white"/>
<br>
**Communication**
<br>
<img src="https://img.shields.io/badge/Notion-0000000?style=flat-square&logo=notion&logoColor=white"/>
<br>

## 게임 화면 구성
| 메인 메뉴  |  플레이어 시점   |
| :-------------------------------------------: | :------------: |
|  <img width="329" src="https://github.com/DimBlessing/DimBlessing/assets/47944912/d7f3d965-e389-4b63-9743-ebb0ea25b2e0"/> |  <img width="329" src="https://github.com/DimBlessing/DimBlessing/assets/47944912/e553fb27-ce64-41a1-a60d-5c8469c550a6"/>|  
| 맵   |  NPC   |  
| <img width="329" src="https://github.com/DimBlessing/DimBlessing/assets/47944912/54efa987-2588-4f44-8e48-e739366bfa39"/>   |  <img width="329" src="https://github.com/DimBlessing/DimBlessing/assets/47944912/3d744bd9-2727-463d-a26d-41db1f7d0c0c"/>     |
| 맵 구조변경 기능   |  플레이어 시각효과   |  
| <img width="329" src="https://github.com/DimBlessing/DimBlessing/assets/47944912/8aca8abe-a140-4b42-9f5e-a3794a3c3d01"/>   |  <img width="329" src="https://github.com/DimBlessing/DimBlessing/assets/47944912/a7e15adc-cb0f-45cc-9adf-10aa573b853b"/>     |



## 주요 기능
### 플레이어의 아이템 사용에 의한 맵 구조변경 시스템(벽 뜯기)
- 방탈출 공포게임에 비선형적 진행방식을 통해 플레이의 자유도 향상을 목표로 기능 도입
- 게임 맵(건물)의 벽 오브젝트를 이동시킴으로써 NPC 회피에 활용하거나 새로운 게임 진행 경로 생성
- 인게임에서 습득할 수 있는 망치 아이템을 통해 기능 사용 가능(횟수 제한)

### 적대적 NPC
- 공포게임에서 플레이어에게 공포감을 유발할 수 있는 외형의 NPC 모델 추가
- 맵 내를 자동적으로 순찰, 플레이어를 인식할 시 추적 및 공격하는 기능 구현(Raycast 기반 인식)
- 맵 구조변경 시스템에 의한 맵 수정에 대응한 Runtime Navmesh 

### 퍼즐 및 단서
- 게임 진행 및 클리어를 위한 컨텐츠로써 퍼즐을 맵 내에 배치
- 습득한 단서를 통한 퍼즐 해결 및 인게임 스토리 진행
- 정신력 회복 아이템 등 습득하여 인벤토리에서 사용할 수 있는 아이템을 배치
  
### 플레이어 조작 및 시각/사운드 효과
- PC환경에서 플레이어 캐릭터 조작 기능 구현, 조작에 따른 플레이어 캐릭터 애니메이션 설정
- 적대적 NPC에 대한 공포감 극대화를 위해 근접, 조우, 피격 등 상황에 따른 시각적 효과 구현
- 인게임 요소 상호작용에 따른 사운드 효과 재생











