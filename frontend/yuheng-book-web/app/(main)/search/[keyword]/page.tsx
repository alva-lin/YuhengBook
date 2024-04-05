export default function Page({
  params: { keyword },
}: {
  params: {
    keyword: string;
  };
}) {
  keyword = decodeURI(keyword);

  return <div>{keyword}</div>;
}
