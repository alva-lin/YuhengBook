'use client';

import { useCallback, useEffect, useState } from 'react';

import { useParams, useRouter } from 'next/navigation';

import { Group, NavLink, TextInput } from '@mantine/core';
import { getHotkeyHandler } from '@mantine/hooks';

export function Header() {
  // const [opened, { toggle }] = useDisclosure(false);
  const [text, setText] = useState<string | undefined>();
  const router = useRouter();

  const params = useParams<{ keyword?: string }>();
  useEffect(() => {
    if (params.keyword) {
      const keyword = decodeURI(params.keyword);
      setText(keyword);
    }
  }, [params]);

  const onSearch = useCallback(() => {
    router.push(`/search/${text}`);
  }, [router, text]);

  return (
    <div className="m-auto h-full max-w-[1200px] flex justify-between items-center px-2">
      <div className="flex gap-2 items-center">
        {/*<Burger opened={opened} onClick={toggle} size="sm" hiddenFrom="sm" />*/}
        <NavLink
          href="/"
          label="玉衡小说"
          leftSection={
            <span className="i-fluent-book-24-regular w-7 h-7" role="img" aria-hidden="true" />
          }
        />
      </div>

      <Group>
        <TextInput
          size="md"
          placeholder="搜索书籍"
          value={text}
          onChange={(e) => setText(e.target.value)}
          onKeyDown={getHotkeyHandler([['Enter', onSearch]])}
        />
      </Group>
    </div>
  );
}
