let tabDetails;
let isVerticalTabCreateFunctionExecuted = false;
const verticalTab2Container = document.querySelector('.vertical-tab-2-container');
const verticalTab2Group = document.querySelector('.vertical-tab-component-2');
verticalTab2Group.append(verticalTab2Container);
let selectedIndex = 0;
let isClicked = false;

function createTabs() {
    const verticalTabDetails2group = document.querySelector('.vertical-Tab-Details-2-group');
    const verticalTab2Heading = document.querySelector('.vertical-tab-2-heading');
    const verticalTabDetails = document.querySelectorAll('.vertical-tab-details');
    const verticalTabDetailsArr = Array.from(verticalTabDetails).map((tab) => {
        let title = "";
        let content = "";
        let image = "";
        Array.from(tab.children).forEach((para, i) => {
            if (i == 0)
                title = para.innerHTML;
            else if (i === 1) {
                image = para.innerHTML;
            } else
                content += para.innerHTML + '\n';
        });
        return {
            title,
            image,
            content
        };
    });
    tabDetails = {
        heading: verticalTab2Heading.innerHTML,
        tabData: verticalTabDetailsArr
    };
    document.getElementById('heading-vertical-tab').innerHTML = verticalTab2Heading.innerHTML;
    verticalTabDetails2group.remove();
    const tabContainer = document.getElementById('tabContainer');
    const tabList = document.createElement('div');
    tabList.classList.add('tab-list');
    const tabListInner = document.createElement('div');
    tabListInner.classList.add('tab-list-inner');
    tabDetails ? .tabData.forEach((tab, index) => {
        const tabElement = document.createElement('div');
        const imgElement = document.createElement('div');
        imgElement.classList.add('tab-image');
        imgElement.innerHTML = tab.image;
        tabElement.classList.add('tab');
        tabElement.textContent = tab.title;
        tabElement.appendChild(imgElement);
        tabElement.addEventListener('click', () => {
            selectTab(index);
            selectedIndex = index;
            isClicked = true;
        });
        tabListInner.appendChild(tabElement);
    });
    tabList.appendChild(tabListInner);
    tabContainer.appendChild(tabList);
    const tabContent = document.createElement('div');
    tabContent.classList.add('tab-content');
    tabContainer.appendChild(tabContent);
    selectTab(0);
}

function selectTab(index) {
    const tabs = document.querySelectorAll('.tab');
    const tabContent = document.querySelector('.tab-content');
    const selectedTab = tabs[index];
    tabs.forEach(tab => tab.classList.remove('selected'));
    selectedTab.classList.add('selected');
    var content = tabDetails.tabData[index].content.replace(/\n/g, '<br>');
    var linkedContent = content.replace(/#anchortag#/g, '</a>').replace(/#anchortag/g, "<a href=\"").replace(/anchortag#/g, "\">");
    tabContent.innerHTML = `<h4 class="h4-tabs">${tabDetails.tabData[index].title}</h4>` + '<p class="tab-content-text">' + linkedContent + '</p>';
    const screenWidth = window.innerWidth;
    if (screenWidth < 768 && !isClicked) {
        scrollChild();
    }
}
window.addEventListener('scroll', () => {
    if (window.pageYOffset > 700 && !isVerticalTabCreateFunctionExecuted) {
        isVerticalTabCreateFunctionExecuted = true;
        createTabs();
        const screenWidth = window.innerWidth;
        if (screenWidth < 768) {
            setInterval(autoScrollTabs, 6000);
        }
    }
});

function autoScrollTabs() {
    const tabs = document.querySelectorAll('.tab');
    if (isClicked == false) {
        if (selectedIndex >= tabs.length - 1) {
            selectedIndex = 0;
            selectTab(0);
        } else {
            selectedIndex += 1;
            selectTab(selectedIndex);
        }
    }
}

function scrollChild() {
    const parent = document.querySelector('.tab-list');
    const childWidth = document.querySelector('.tab.selected').offsetWidth + 20;
    if (selectedIndex <= 0) {
        parent.scroll({
            left: 0,
            behavior: 'smooth'
        });
    } else {
        parent.scroll({
            left: childWidth * selectedIndex,
            behavior: 'smooth'
        })
    }
}