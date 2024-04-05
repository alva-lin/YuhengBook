'use client';

import { Text, Container } from '@mantine/core';
import classes from './footer.module.css';
import { Logo } from '@/components/Logo/logo';

const data = [
  {
    title: '帮助',
    links: [
      {
        label: '没有想看的小说',
        link: '#',
      },
      {
        label: '章节报错',
        link: '#',
      },
      {
        label: '页面报错',
        link: '#',
      },
      {
        label: '其他问题',
        link: '#',
      },
    ],
  },
  {
    title: '关于',
    links: [
      {
        label: '玉衡小说',
        link: '#',
      },
      {
        label: '赞助我',
        link: '#',
      },
      {
        label: '联系方式',
        link: '#',
      },
      {
        label: '本站源码',
        link: '#',
      },
    ],
  },
];

export function Footer() {
  const groups = data.map((group) => {
    const links = group.links.map((link, index) => (
      <Text<'a'>
        key={index}
        className={classes.link}
        component="a"
        href={link.link}
        onClick={(event) => event.preventDefault()}
      >
        {link.label}
      </Text>
    ));

    return (
      <div className="w-40" key={group.title}>
        <Text className={classes.title}>{group.title}</Text>
        {links}
      </div>
    );
  });

  return (
    <div>
      <div className={classes.spacer}></div>
      <div className={classes.wrapper}>
        <Container className="flex flex-col gap-4 items-center md:flex-row md:justify-between md:items-start">
          <div className="flex flex-col gap-2 items-center md:items-start">
            <Logo />
            <Text size="xs" c="dimmed" className="mt-1">
              纯净小说阅读站点
            </Text>
          </div>
          <div className="flex">{groups}</div>
        </Container>

        <Container className="flex flex-col justify-between items-center mt-4 py-4 border-t md:flex-row">
          <Text c="dimmed" size="sm">
            © 2024 yuheng.site. All rights reserved.
          </Text>

          {/*<Group gap={0} className="mt-2 md:mt-0" justify="flex-end" wrap="nowrap">*/}
          {/*  <ActionIcon size="lg" color="gray" variant="subtle">*/}
          {/*    <span*/}
          {/*      className="i-fluent-book-24-regular w-[18px] h-[18px]"*/}
          {/*      role="img"*/}
          {/*      aria-hidden="true"*/}
          {/*    />*/}
          {/*  </ActionIcon>*/}
          {/*</Group>*/}
        </Container>
      </div>
    </div>
  );
}
