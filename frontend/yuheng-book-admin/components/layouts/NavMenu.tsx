import { ScrollArea } from '@mantine/core';
import { IconGauge, IconNotes } from '@tabler/icons-react';
import { LinksGroup } from '@/components/layouts/NavMenuLinkGroup';
import classes from './NavMenu.module.css';

const menuList = [
  {
    label: '控制台',
    icon: IconGauge,
    href: '/',
  },
  {
    label: '书籍管理',
    icon: IconNotes,
    initiallyOpened: false,
    href: '/book',
  },
  // {
  //   label: '系统设置',
  //   icon: IconAdjustments,
  //   links: [
  //     {
  //       label: '用户管理',
  //       href: '/',
  //     },
  //   ],
  // },
];

export function NavMenu() {
  const links = menuList.map((item) => <LinksGroup {...item} key={item.label} />);

  return (
    <>
      {/*<div className={classes.header}></div>*/}
      <ScrollArea className={classes.links}>
        <div className={classes.linksInner}>{links}</div>
      </ScrollArea>
      {/*<div className={classes.footer}></div>*/}
    </>
  );
}
