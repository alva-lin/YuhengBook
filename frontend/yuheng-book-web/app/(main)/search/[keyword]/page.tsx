'use client';

import { useCallback } from 'react';

import Link from 'next/link';

import { Card, LoadingOverlay, Pagination, Text } from '@mantine/core';
import { keepPreviousData, useQuery } from '@tanstack/react-query';

import { useSearchParams, useRouter } from 'next/navigation';
import { Api } from '@/lib/api';

export default function Page({
  params: { keyword },
}: {
  params: {
    keyword: string;
  };
}) {
  keyword = decodeURI(keyword);

  const searchParams = useSearchParams();
  const router = useRouter();

  const page = parseInt(searchParams.get('page') ?? '1', 10);
  const pageSize = 10;

  const { data, isFetching } = useQuery({
    queryKey: ['books', keyword, pageSize, page],
    queryFn: () =>
      Api.Books.Book.getList({
        page,
        pageSize,
        keyword,
      }),
    placeholderData: keepPreviousData,
  });

  const setPageAndNavigate = useCallback(
    (newPage: number) => {
      const url = `/search/${keyword}?page=${newPage}`;
      router.push(url);
    },
    [searchParams, router]
  );

  const items = (data?.data ?? []).map((item) => (
    <Card shadow="md" padding="md" maw={800} miw={300} component={Link} href={`/book/${item.id}`}>
      <div className="flex justify-between mb-4">
        <Text fw={500} size="md">
          {item.name}
        </Text>
        {item.lastChapter && (
          <Text
            mt="xs"
            c="dimmed"
            size="sm"
            component={Link}
            href={`/book/${item.id}/chapter/${item.lastChapter.order}`}
          >
            {item.lastChapter.title}
          </Text>
        )}
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
      <div className="flex flex-col my-12 items-center justify-center">
        <div className="flex flex-col gap-4 items-center">
          <LoadingOverlay visible={isFetching} loaderProps={{ children: '查询中...' }} />
          {data && (
            <>
              <Pagination
                total={data.totalPage}
                value={page}
                onChange={setPageAndNavigate}
                siblings={1}
                withEdges
              />
              <div className="flex flex-col gap-4 justify-center ">{items}</div>
              <Pagination
                total={data.totalPage}
                value={page}
                onChange={setPageAndNavigate}
                siblings={1}
                withEdges
              />
            </>
          )}
        </div>
      </div>
    </>
  );
}
