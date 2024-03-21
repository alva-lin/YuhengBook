'use client';

import { Card, LoadingOverlay, Pagination, Text, TextInput } from '@mantine/core';
import { useDebouncedValue } from '@mantine/hooks';
import { keepPreviousData, useQuery } from '@tanstack/react-query';
import Link from 'next/link';
import { useEffect, useState } from 'react';
import { Api } from '@/lib/api';

export default function HomePage() {
  const pageSize = 3;
  const [page, setPage] = useState(1);
  const [text, setText] = useState<string | undefined>();
  const [keyword] = useDebouncedValue(text, 300);

  const {
    data,
    isFetching,
    refetch,
  } = useQuery({
    queryKey: ['books', keyword, pageSize, page],
    queryFn: () => Api.Books.Book.getList({
      page,
      pageSize,
      keyword,
    }),
    placeholderData: keepPreviousData,
    enabled: false,
  });

  useEffect(() => {
    if (keyword) {
      setPage(1);
      refetch();
    }
  }, [keyword, refetch]);

  const items = (data?.data ?? []).map((item) => (
    <Card
      shadow="md"
      padding="md"
      maw={800}
      miw={500}
      component={Link}
      href={`/book/${item.id}`}
    >
      <div className="flex justify-between mb-4">
        <Text fw={500} size="md">
          {item.name}
        </Text>
        {item.lastChapter &&
          <Text
            mt="xs"
            c="dimmed"
            size="sm"
            component={Link}
            href={`/book/${item.id}/chapter/${item.lastChapter.order}`}>
            {item.lastChapter.title}
          </Text>
        }
      </div>
      <div>
        <Text lineClamp={3} c="dimmed">
          {item.description}
        </Text>
      </div>
    </Card>
  ));

  return (
    <>
      <div className="flex flex-col mt-32 items-center justify-center">
        <div className="text-center font-black tracking-widest text-6xl flex justify-center gap-2">
          <span className="i-fluent-book-24-regular" role="img" aria-hidden="true" />
          <span>玉衡小说网</span>
        </div>
        <div className="flex flex-col gap-4 mt-32 items-center">
          <TextInput
            w={480}
            size="xl"
            placeholder="键入书名以搜索，宁可少字也不要多字..."
            value={text}
            onChange={(e) => setText(e.target.value)} />
          <LoadingOverlay visible={isFetching} loaderProps={{ children: '查询中...' }} />
          {data && (<>
            <Pagination
              total={data.totalPage}
              value={page}
              onChange={setPage}
              siblings={1}
              withEdges />
            <div className="flex flex-col gap-4 justify-center ">
              {items}
            </div>
                    </>)}
        </div>
      </div>
    </>
  );
}
